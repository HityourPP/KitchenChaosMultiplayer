using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
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
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

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
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiPlayer.Instance.GetPlayerName();
        //当玩家名字改变时，设置新的玩家名字
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiPlayer.Instance.SetPlayerName(newText);
        });
        
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobbyOnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobbyOnLobbyListChanged;
    }

    private void KitchenGameLobbyOnLobbyListChanged(object sender, KitchenGameLobby.LobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if(child == lobbyTemplate)continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
