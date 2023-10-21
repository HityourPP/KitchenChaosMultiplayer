using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playSingleButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {//() =>为lambda表达式
        playSingleButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.LobbyScene);
        }));         
        playButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.LobbyScene);
        }));       
        playMultiplayerButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.LobbyScene);
        }));
        quitButton.onClick.AddListener((() =>
        {
            Application.Quit();
        }));
        Time.timeScale = 1f;
    }
}
