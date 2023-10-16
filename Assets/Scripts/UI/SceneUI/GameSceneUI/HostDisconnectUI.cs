using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        playAgainButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.Shutdown();    //退出场景时关闭网络连接
            Loader.Load(Loader.Scene.MainMenu);
        }));
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        Hide();
    }

    private void OnDestroy()
    {
        //场景切换时会销毁这些物品，但不会取消订阅，这里手动设置
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManagerOnClientDisconnectCallback;
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {   //服务端正在关闭
            Show();
        }   
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
