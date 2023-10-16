 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVirtual : MonoBehaviour
{//实现动画效果
    private Animator anim;
    [SerializeField] private ContainerCounter containerCounter;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        containerCounter.ShowVistualEffect += ContainerCounterOnShowVistualEffect;
    }

    private void ContainerCounterOnShowVistualEffect(object sender, EventArgs e)
    {
        anim.SetTrigger("OpenClose");
    }
}
