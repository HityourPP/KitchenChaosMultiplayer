using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class KitchenGameMultiPlayer : NetworkBehaviour
{
    public static KitchenGameMultiPlayer Instance { get; private set; }
    //最大一起游玩的玩家数设置为4
    private const int MAX_PLAYER_AMOUNT = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    
    //保存物品数据的列表
    [SerializeField] private KitchenObjectsList_SO kitchenObjectSoList;
    
    [SerializeField] private List<Color> playerColorList;
    
    private NetworkList<PlayerData> playerDataNetworkList;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnOnListChanged;
    }

    private void PlayerDataNetworkListOnOnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        //添加连接是否批准的函数
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManagerConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {   //移除对应玩家数据
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManagerOnClientConnectedCallback(ulong clientId)
    {
        //当有玩家连接时，将对应客户端id保存在玩家数据中
        playerDataNetworkList.Add(new PlayerData()
        {
            clientId =  clientId,
            colorId = GetFirstUnusedColorId()
        });
    }

    private void NetworkManagerConnectionApprovalCallback(
        NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //若服务端不在选人界面，则客户端无法加入
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is already start !!!";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full !!!";
            return;
        } 
        connectionApprovalResponse.Approved = true;
            // if (GameManager.Instance.IsWaitingToStart())
            // {
            //     //在等待开始时，同意进行连接
            //     connectionApprovalResponse.Approved = true;
            //     //允许创建角色
            //     connectionApprovalResponse.CreatePlayerObject = true;   
            // }
            // else
            // {   //其他时间拒绝连接
            //     connectionApprovalResponse.Approved = false;
            // }
    }

    public void StartClient()
    {
        //启用正在连接的事件
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj) 
    { 
        //连接失败调用失败的事件
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// 生成新的网络对象
    /// </summary>
    public void SpawnNewKitchenObject(KitchenObjects_SO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {   //调用Rpc函数
        SpawnNewKitchenObjectServerRpc(GetKitchenObjectSoIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }
    /// <summary>
    /// 这里使用NetworkObjectReference(引用网络对象)
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SpawnNewKitchenObjectServerRpc(int index,NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        //获取物品数据
        KitchenObjects_SO kitchenObjects_SO = GetKitchenObjectsSoFromIndex(index);
        //生成物品
        Transform kitchenObjectSpawn = Instantiate(kitchenObjects_SO.prefabs);
        //获取生成的物品的networkObject组件
        NetworkObject kitchenNetworkObject = kitchenObjectSpawn.GetComponent<NetworkObject>();
        //设置随场景加载而销毁（默认）
        kitchenNetworkObject.Spawn(true);
        //获取KitchenObject代码组件
        KitchenObject kitchenObject = kitchenObjectSpawn.GetComponent<KitchenObject>();
        //获取父对象引用的NetworkObject组件
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        //获取组件
        IKitchenObjectParent kitchenObjectParent =
            kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        //设置父对象
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    /// <summary>
    /// 获取对应序号
    /// </summary>
    public int GetKitchenObjectSoIndex(KitchenObjects_SO kitchenObjectSO)
    {
        return kitchenObjectSoList.kitchenObjectsSoList.IndexOf(kitchenObjectSO);
    }
    /// <summary>
    /// 按照序号获取对应物品数据
    /// </summary>
    public KitchenObjects_SO GetKitchenObjectsSoFromIndex(int index)
    {
        return kitchenObjectSoList.kitchenObjectsSoList[index];
    }
    /// <summary>
    /// 销毁物品
    /// </summary>
    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        DestroyKitchenObjectClientRpc(kitchenObjectNetworkObjectReference);
        //摧毁对象只能在服务端执行
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void DestroyKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        //清除父对象可以在客户端运行
        kitchenObject.ClearKitchenObjectParent();
    }
    /// <summary>
    /// 获取对应序号的玩家是否连接
    /// </summary>
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    /// <summary>
    /// 从用户端id中获取玩家数据
    /// </summary>
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }    
    /// <summary>
    /// 从用户端id获取玩家数据序号
    /// </summary>
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }
    /// <summary>
    /// 获取当前用户端的用户数据
    /// </summary>
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    /// <summary>
    /// 获取对应序号的玩家数据
    /// </summary>
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];  
    }
    /// <summary>
    /// 获取对应序号的玩家颜色
    /// </summary>
    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {   //该颜色已被使用
            return;
        }
        //获取对应玩家数据并重新赋值，这里不能直接playerDataNetworkList[playerDataIndex].colorId = colorId;会报错
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {   //已经被使用
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// 设置踢人函数
    /// </summary>
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        //手动出发断开连接函数
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
