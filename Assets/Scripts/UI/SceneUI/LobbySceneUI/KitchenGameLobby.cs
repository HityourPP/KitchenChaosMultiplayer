using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }
    private Lobby joinedLobby;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
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
        //为了确保游戏不会中断运行,使用Try catch语句
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiPlayer.MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });
            KitchenGameMultiPlayer.Instance.StartHost();
            //使用多人联机下的加载场景方式
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    /// <summary>
    /// 快速加入，启动客户端加入
    /// </summary>
    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    /// <summary>
    /// 通过房间码进入房间
    /// </summary>
    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            KitchenGameMultiPlayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
