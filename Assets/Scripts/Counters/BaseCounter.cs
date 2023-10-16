using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour,IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlaceHere;  //设置放置音效
    public static void ResetStaticData()
    {
        OnAnyObjectPlaceHere = null;    //由于是静态资源，在加载场景的时候不会销毁，需要我们手动销毁
    }
    [SerializeField] protected Transform topCountersPos;
    protected KitchenObject kitchenObject;
    public virtual void Interact(PlayerController player)
    {
        // Debug.LogError("Interact Error!");
    }

    public virtual void InteractAlternate(PlayerController player)
    {
        // Debug.LogError("Interact Error!");
    }
    
    //将接口实现写在父类中
    public Transform GetTopCounterPos()
    {
        return this.topCountersPos;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject)
        {
            OnAnyObjectPlaceHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return this.kitchenObject;
    }

    public void ClearKitchenObject() 
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()  //判断是否有物体
    {
        return this.kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
