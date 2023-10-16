using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener((() =>
        {
            //先关闭网络连接，再加载到主场景
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        }));
        readyButton.onClick.AddListener((() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        }));
    }
}
