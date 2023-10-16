using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounter : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    private GameObject virtualGameObject;
    private void Start()
    {        
        virtualGameObject = transform.GetChild(0).gameObject;   //要显示的物体
        if (PlayerController.LocalInstance != null)             //对于主机上的玩家，单例是非空的，所以直接添加
        {
            PlayerController.LocalInstance.OnSelectedCounterChanged += SelectedCounterChanged;//为创建的包含类参数的事件添加执行函数
        }
        //对于客户端的玩家，单例还未生成，所以将这个函数添加到委托中，便于生成后再添加
        else
        {
            PlayerController.OnAnyPlayerSpawned += PlayerControllerOnAnyPlayerSpawned; 
        }
    }

    private void PlayerControllerOnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnSelectedCounterChanged -= SelectedCounterChanged; //避免重复添加
            PlayerController.LocalInstance.OnSelectedCounterChanged += SelectedCounterChanged;
        }
    }

    private void SelectedCounterChanged(object sender, PlayerController.OnSelectedCounter e)
    {
        if (e.selectedCounter == baseCounter)//当事件中的参数与赋值的相同时
        {
            virtualGameObject.SetActive(true); //显示选中状态
        }
        else
        {
            virtualGameObject.SetActive(false);
        }
    }
}
