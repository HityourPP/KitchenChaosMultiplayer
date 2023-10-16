using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountDownUI : MonoBehaviour
{
    private TextMeshProUGUI countDownText;
    private Animator anim;
    
    private int countDownNumber;
    private int preCountDownNumber;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        countDownText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        Hide();
    }

    private void GameManagerOnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountDownStart())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {//Mathf.Ceil(a)返回大于等于a的最小整数
        countDownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountDownStartTimer());        
        countDownText.text = countDownNumber.ToString();
        if (countDownNumber != preCountDownNumber)
        {
            preCountDownNumber = countDownNumber;
            anim.SetTrigger("NumberPop");
            SoundManager.Instance.PlayNumberPopSound();
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
