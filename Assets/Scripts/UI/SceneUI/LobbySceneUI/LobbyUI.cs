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

    private void Awake()
    {
        mainMenuButton.onClick.AddListener((() =>
        {
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
}
