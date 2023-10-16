using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;   
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener((() =>
        {
            GameManager.Instance.PauseGame();
        }));
        mainMenuButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.Shutdown();    //退出场景时关闭网络连接
            Loader.Load(Loader.Scene.MainMenu);
        }));
        optionsButton.onClick.AddListener((() =>
        {
            OptionsUI.Instance.Show();
        }));
    }

    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += LocalGameManagerOnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnPaused += LocalGameManagerOnLocalGameUnPaused;
        Hide();
    }

    private void LocalGameManagerOnLocalGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void LocalGameManagerOnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
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
