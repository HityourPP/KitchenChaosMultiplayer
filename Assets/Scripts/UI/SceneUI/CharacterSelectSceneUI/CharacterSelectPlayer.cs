using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener((() =>
        {
            // Debug.Log("Kick");
            PlayerData playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            KitchenGameMultiPlayer.Instance.KickPlayer(playerData.clientId);
        }));
    }

    private void Start()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiPlayerOnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnPlayerReadyChanged += CharacterSelectReadyOnPlayerReadyChanged;
        //让踢人按钮只在服务端显示
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        if (playerIndex == 0)
        {
            kickButton.gameObject.SetActive(false);
        }

        UpdatePlayer();
    }

    private void OnDestroy()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiPlayerOnPlayerDataNetworkListChanged;
    }

    private void CharacterSelectReadyOnPlayerReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiPlayerOnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    /// <summary>
    /// 更新玩家预制体是否显示
    /// </summary>
    private void UpdatePlayer()
    {
        if (KitchenGameMultiPlayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = KitchenGameMultiPlayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            
            playerVisual.SetPlayerColor(KitchenGameMultiPlayer.Instance.GetPlayerColor(playerData.colorId));

            playerNameText.text = playerData.playerName.ToString();
        }
        else
        {
            Hide();
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
