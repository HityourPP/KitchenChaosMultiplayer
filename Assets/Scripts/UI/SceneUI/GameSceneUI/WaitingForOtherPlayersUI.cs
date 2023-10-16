using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        Hide();
    }
    /// <summary>
    /// 状态改变时隐藏
    /// </summary>
    private void GameManagerOnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountDownStart())
        {
            Hide();
        }
    }

    /// <summary>
    /// 玩家准备时显示
    /// </summary>
    private void GameManagerOnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Show();
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
