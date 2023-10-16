using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTranform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
    
    private void LateUpdate()
    {//让目标转换在所有动作之后
        if(!targetTransform)return;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
