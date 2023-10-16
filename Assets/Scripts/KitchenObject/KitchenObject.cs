using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjects_SO kitchenObjectsSo;

    private IKitchenObjectParent kitchenObjectParent;

    private FollowTranform followTranform;

    protected virtual void Awake()
    {
        followTranform = GetComponent<FollowTranform>();
    }

    public KitchenObjects_SO GetKitchenObjectsSO()
    {
        return this.kitchenObjectsSo;
    }
    
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        // transform.parent = kitchenObjectParent.GetTopCounterPos().transform;    //将该物品放置在新柜台上
        // transform.localPosition = Vector3.zero;
    }
    /// <summary>
    /// 服务端执行，并分发给客户端
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }
    /// <summary>
    /// 客户端执行
    /// </summary>
    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {        
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        //获取组件
        IKitchenObjectParent kitchenObjectParent =
            kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenObjectParent != null)  //将该物品放置的原本的柜台所包含的物品清空
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        if (kitchenObjectParent.HasKitchenObject())
        {   
            Debug.Log("无法放置");
        }
        this.kitchenObjectParent = kitchenObjectParent;    //新的柜台
        
        kitchenObjectParent.SetKitchenObject(this);
        followTranform.SetTargetTransform(kitchenObjectParent.GetTopCounterPos().transform);
    }
    
    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return this.kitchenObjectParent;
    }
    //这里进行了拆分，因为需要分开在服务端和客户端执行
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectParent()
    {
        kitchenObjectParent.ClearKitchenObject();//清空父对象
    }
    
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        plateKitchenObject = null;
        return false;
    }
    
    public static void SpawnNewKitchenObject(KitchenObjects_SO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {//生成一个物品
        KitchenGameMultiPlayer.Instance.SpawnNewKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiPlayer.Instance.DestroyKitchenObject(kitchenObject);
    }
    
}
