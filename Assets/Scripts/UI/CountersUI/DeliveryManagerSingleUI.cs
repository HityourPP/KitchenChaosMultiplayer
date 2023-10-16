using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(Recipe_SO recipeSo)
    {
        recipeName.text = recipeSo.recipeName;
        foreach (Transform child in iconContainer)
        {
            if(child == iconTemplate)continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjects_SO kitchenObjectsSo in recipeSo.kitchenObjectsSoList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectsSo.objectSprite;
            iconTransform.gameObject.SetActive(true);
        }
    }
}
