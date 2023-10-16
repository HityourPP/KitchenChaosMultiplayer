using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectDestroyed;
    public new static void ResetStaticData()
    {
        OnAnyObjectDestroyed = null;    //由于是静态资源，在加载场景的时候不会销毁，需要我们手动销毁
    }
    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())//将角色手中的物品摧毁
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
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
        OnAnyObjectDestroyed?.Invoke(this, EventArgs.Empty);
    }
}
