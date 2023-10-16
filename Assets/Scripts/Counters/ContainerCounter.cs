using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler ShowVistualEffect;
    [SerializeField] private KitchenObjects_SO kitchenObjectSO;

    // private Animator anim;
    // private void Start()
    // {
    //     anim = GetComponent<Animator>();
    // }
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//玩家没有手上没有物体时
        {
            // anim.SetTrigger("OpenClose");    //这里使用事件委托解决了问题
            KitchenObject.SpawnNewKitchenObject(kitchenObjectSO, player); //在产生柜台，直接将产生的物体放在玩家手上
            //由于需要多个使用该功能，增加代码复用性，添加了函数实现
            // Transform objectSpawn = Instantiate(kitchenObjectSO.prefabs);
            // objectSpawn.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
            // ShowVistualEffect?.Invoke(this,EventArgs.Empty);    
            InteractLogicServerRpc();
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
        ShowVistualEffect?.Invoke(this,EventArgs.Empty);    

    }
    
}
