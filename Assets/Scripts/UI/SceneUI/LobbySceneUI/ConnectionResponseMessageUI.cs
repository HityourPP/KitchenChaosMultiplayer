using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
        KitchenGameMultiPlayer.Instance.OnFailedToJoinGame += KitchenGameMultiPlayerOnFailedToJoinGame;
    }

    private void OnDestroy()
    {
        //由于KitchenGameMultiPlayer不会随着场景销毁而销毁，所以这些订阅的事件也不会销毁，这里进行手动销毁
        KitchenGameMultiPlayer.Instance.OnFailedToJoinGame -= KitchenGameMultiPlayerOnFailedToJoinGame;
    }

    private void KitchenGameMultiPlayerOnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        //若没有服务端创建游戏，则文本会为空，这里添加默认文本
        if (messageText.text == "")
        {
            messageText.text = "Fail to connect...";
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
