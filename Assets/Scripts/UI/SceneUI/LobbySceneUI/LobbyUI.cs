using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener((() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenu);
        }));
        createLobbyButton.onClick.AddListener((() =>
        {
            lobbyCreateUI.Show();
        }));
        quickJoinButton.onClick.AddListener((() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        }));
        joinCodeButton.onClick.AddListener((() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        }));
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiPlayer.Instance.GetPlayerName();
        //当玩家名字改变时，设置新的玩家名字
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiPlayer.Instance.SetPlayerName(newText);
        });
    }
}
