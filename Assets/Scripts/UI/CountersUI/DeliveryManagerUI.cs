using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

    [SerializeField] private Transform recipeTemplate;
    [SerializeField] private Transform container;
    
    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DelieverManager.Instance.OnRecipeSpawn += InstanceOnRecipeSpawn;
        DelieverManager.Instance.OnRecipeComplete += InstanceOnRecipeComplete;     
        UpdateVirual();
    }

    private void InstanceOnRecipeSpawn(object sender, EventArgs e)
    {
        UpdateVirual();
    }

    private void InstanceOnRecipeComplete(object sender, EventArgs e)
    {
        UpdateVirual();
    }

    private void UpdateVirual()
    {
        foreach (Transform child in container)  //每次生成前，先将之前生成的删去
        {
            if(child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (var recipeSO in DelieverManager.Instance.GetWaitRecipeList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
            
        }
    }
}
