using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
        Show();
    }

    private void GameManagerOnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    // private void GameManagerOnStateChanged(object sender, EventArgs e)
    // {
    //     if (GameManager.Instance.IsCountDownStart())
    //     {
    //         Hide();
    //     }
    // }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
