using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    [SerializeField] private KitchenObjects_SO plateKitchenObjectsSo;
    [SerializeField] private GameObject[] platesVirtual;
    
    private float spawnPlateTime;
    private float spawnPlateTimeMax = 4f;
    private int currentPlateAmount;
    private int maxPlateAmount = 4;

    private void Start()
    {
        spawnPlateTime = 0f;
        currentPlateAmount = 0;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnPlateTime += Time.deltaTime;
        if (spawnPlateTime >= spawnPlateTimeMax)
        {
            spawnPlateTime = 0f;
            if (GameManager.Instance.IsGamePlaying() && currentPlateAmount < maxPlateAmount)
            {                
                SpawnPlateServerRpc();
            }
        }
    }
    /// <summary>
    /// 同步盘子生成
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesVirtual[currentPlateAmount].SetActive(true);
        currentPlateAmount++;
    }

    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//当玩家手上没有物体时
        {
            if (currentPlateAmount > 0)
            {
                KitchenObject.SpawnNewKitchenObject(plateKitchenObjectsSo, player);   
                InteractLogicServerRpc();
            }
        }
    }
    /// <summary>
    /// 在服务器执行并广播给客户端
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() 
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesVirtual[currentPlateAmount-1].SetActive(false);
        currentPlateAmount--;
    }
}
