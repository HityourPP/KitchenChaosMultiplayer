using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVirtual : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCuttingVirtual += CuttingCounterOnOnCuttingVirtual;
    }

    private void CuttingCounterOnOnCuttingVirtual(object sender, EventArgs e)
    {
        anim.SetTrigger("Cut");
    }
}
