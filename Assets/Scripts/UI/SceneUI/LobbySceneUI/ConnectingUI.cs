using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        Hide();
        //为事件添加函数
        KitchenGameMultiPlayer.Instance.OnTryingToJoinGame += KitchenGameMultiPlayerOnTryingToJoinGame;
        KitchenGameMultiPlayer.Instance.OnFailedToJoinGame += KitchenGameMultiPlayerOnFailedToJoinGame;
    }
    private void KitchenGameMultiPlayerOnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
    }
    private void KitchenGameMultiPlayerOnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }
    private void OnDestroy()
    {
        //由于KitchenGameMultiPlayer不会随着场景销毁而销毁，所以这些订阅的事件也不会销毁，这里进行手动销毁
        KitchenGameMultiPlayer.Instance.OnTryingToJoinGame -= KitchenGameMultiPlayerOnTryingToJoinGame;
        KitchenGameMultiPlayer.Instance.OnFailedToJoinGame -= KitchenGameMultiPlayerOnFailedToJoinGame;
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
