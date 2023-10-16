using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PauseMultiPlayerUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnMultiPlayerGamePaused += GameManagerOnMultiPlayerGamePaused; 
        GameManager.Instance.OnMultiPlayerGameUnpaused += GameManagerOnMultiPlayerGameUnpaused; 
        Hide();
    }

    private void GameManagerOnMultiPlayerGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManagerOnMultiPlayerGamePaused(object sender, EventArgs e)
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
