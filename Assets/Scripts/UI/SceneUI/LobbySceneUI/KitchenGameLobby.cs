using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class KitchenGameLobby : MonoBehaviour
{
    public const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static KitchenGameLobby Instance { get; private set; }
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler<LobbyListChangedEventArgs> OnLobbyListChanged;

    public class LobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }
    /// <summary>
    /// 每隔15s提示大厅，让其处于活动状态
    /// </summary>
    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = 15f;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    /// <summary>
    /// 定期更新大厅列表
    /// </summary>
    private void HandlePeriodicListLobbies()
    {
        //当还未加入大厅，并且已经初始化登录状态后，进行持续更新列表，还需要添加在LobbyScene这个限制条件
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
        {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer < 0f)
            {
                listLobbiesTimer = 3f;
                ListLobbies();
            }     
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            //设置查询大厅时的选项设置,这里使用分页查询的选项设置
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>()
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this,new LobbyListChangedEventArgs()
            {
                lobbyList =  queryResponse.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// 初始化unity身份验证 ，async将方法标记为异步   
    /// </summary>
    private async void InitializeUnityAuthentication()
    {
        //若已经初始化了，则不再进行初始化
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            //为了让同一设备在加载场景时，加载不同的形象，这里设置随机的配置
            InitializationOptions initializationOptions = new InitializationOptions();
            //下面的在最后不需要在进行设置随机了,暂时注释掉
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());
            //异步初始化游戏服务，异步方法在碰到await表达式之前都是使用同步的方式执行
            await UnityServices.InitializeAsync(initializationOptions);
            //触发匿名登录过程。这将生成或使用存储的会话令牌来访问帐户。
            await AuthenticationService.Instance.SignInAnonymouslyAsync();            
        }
    }
    /// <summary>
    /// 创建大厅,启动服务端进行创建
    /// </summary>
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        //为了确保游戏不会中断运行,使用Try catch语句
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });
            //分配relay
            Allocation allocation = await AllocateRelay();
            
            string relayJoinCode = await GetRelayJoinCode(allocation);
            
            //将relayJoinCode存储在lobby数据中
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {//DataObject.VisibilityOptions.Member设置只对成员可见
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            
            //设置分配的delay,dtls是一种连接加密类型
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(allocation, "dtls"));
            
            KitchenGameMultiPlayer.Instance.StartHost();
            //使用多人联机下的加载场景方式
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    /// <summary>
    /// 分配Relay
    /// </summary>
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            //这里的数目减一是不包括服务端
            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Console.WriteLine(e);
            return default;
        }
    }
    /// <summary>
    /// 获取加入relay的代码
    /// </summary>
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Console.WriteLine(e);
            return default;
        }
    }
    /// <summary>
    /// 通过加入代码加入到分配的relay中
    /// </summary>
    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Console.WriteLine(e);
            return default;
        }
    }

    /// <summary>
    /// 快速加入，启动客户端加入
    /// </summary>
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            //获取加入代码
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            //加入到relay中
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    /// <summary>
    /// 通过房间码进入房间
    /// </summary>
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            //获取加入代码
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            //加入到relay中
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }   
    /// <summary>
    /// 通过id进入房间
    /// </summary>
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            //获取加入代码
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            //加入到relay中
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    //下面添加离开大厅页面后的清理函数
    /// <summary>
    /// 从大厅场景跳转到游戏场景，调用删除大厅，下面的函数
    /// </summary>
    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {

                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    /// <summary>
    /// 从大厅页面退回到主场景，调用离开大厅的函数
    /// </summary>
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }    
    /// <summary>
    /// 踢人时调用下面函数
    /// </summary>
    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
