using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;

public class ClearCounter : BaseCounter //类只能继承一个，接口可实现多个
{
    // [SerializeField] private ClearCounter secondCounter;//测试函数
    // public bool testing; 
    // private void Update()
    // {
    //     if (testing && Input.GetKeyDown(KeyCode.K)) //测试放置
    //     {
    //         if (kitchenObject != null)
    //         {
    //             kitchenObject.SetKitchenObjectParent(secondCounter);
    //         }
    //     }
    // }
    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())//当玩家手上没有物体时
        {
            if (HasKitchenObject()) //若柜台上有物体
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else 
        {
            if (HasKitchenObject())//若柜台上有物品
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))    //当玩家手中携带盘子时
                {
                    if(plateKitchenObject.TryAddKitchenObject(GetKitchenObject().GetKitchenObjectsSO())) //将柜台中的物品添加到盘子中
                    {//若添加成功，则销毁物品
                        //使用新的销毁方式
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        // GetKitchenObject().DestroySelf();//将柜台上的物体销毁
                    } 
                }
                else if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                {
                    if(plateKitchenObject.TryAddKitchenObject(player.GetKitchenObject().GetKitchenObjectsSO())) //将柜台中的物品添加到盘子中
                    {//若添加成功，则销毁物品
                        //使用新的销毁方式
                        KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        // player.GetKitchenObject().DestroySelf();//将柜台上的物体销毁
                    }         
                }
            }

            else if (!HasKitchenObject())    //若玩家手中有物体，但柜台上没有
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
    }


}
