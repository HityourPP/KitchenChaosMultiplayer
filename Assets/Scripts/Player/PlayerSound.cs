using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private PlayerController playerController;
    private float footStepTimer;
    private float footStepTimerMax = .1f;
    private float volume = 1.5f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0)
        {            
            footStepTimer = footStepTimerMax;
            if (playerController.isWalking)
            {
                SoundManager.Instance.PlayFootStepSound(playerController.transform.position, volume);                
            }
        }
    }
}
