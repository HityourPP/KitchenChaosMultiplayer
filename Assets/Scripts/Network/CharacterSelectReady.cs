using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDictionary;  //记录多人玩家的准备情况
    public event EventHandler OnPlayerReadyChanged;
    
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    /// <summary>
    /// 多人游戏中设置玩家准备
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {   
        //serverRpcParams.Receive.SenderClientId为接入端口发送的端口id
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool ifAllPlayersReady = true;
        //检测所有玩家是否已经准备
        //NetworkManager.Singleton.ConnectedClientsIds为已连接的端口id
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {//有玩家还未准备
                ifAllPlayersReady = false;
                break;
            } 
        }
        // Debug.Log(ifAllPlayersReady);
        //所有玩家准备完毕，进入游戏场景
        if (ifAllPlayersReady)
        {
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// 判断对应玩家是否准备
    /// </summary>
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
} 
