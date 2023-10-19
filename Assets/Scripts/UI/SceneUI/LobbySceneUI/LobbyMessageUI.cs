using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
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
        
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobbyOnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobbyOnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobbyOnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobbyOnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobbyOnJoinFailed;
    }

    private void KitchenGameLobbyOnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed To join Lobby!");
    }

    private void KitchenGameLobbyOnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find a Lobby To Join!");
    }

    private void KitchenGameLobbyOnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void KitchenGameLobbyOnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed To create Lobby!");
    }

    private void KitchenGameLobbyOnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }
    private void KitchenGameMultiPlayerOnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect!");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void OnDestroy()
    {
        //由于KitchenGameMultiPlayer不会随着场景销毁而销毁，所以这些订阅的事件也不会销毁，这里进行手动销毁
        KitchenGameMultiPlayer.Instance.OnFailedToJoinGame -= KitchenGameMultiPlayerOnFailedToJoinGame;
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
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
