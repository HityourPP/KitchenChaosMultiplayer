using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener((() =>
        {
            Debug.Log("Host");
            // NetworkManager.Singleton.StartHost();
            KitchenGameMultiPlayer.Instance.StartHost();
            Hide();
        }));
        startClientButton.onClick.AddListener((() =>
        {
            Debug.Log("Client");
            // NetworkManager.Singleton.StartClient();
            KitchenGameMultiPlayer.Instance.StartClient();
            Hide();
        }));
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
