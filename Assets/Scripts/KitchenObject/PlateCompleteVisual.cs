using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjetSO_GameObject
    {
        public KitchenObjects_SO kitchenObjectSo;
        public GameObject gameObject;
    }
    
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjetSO_GameObject> listKitchenObjetSoGameObjects;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnOnIngredientAdded;
        foreach (var kitchenObject in listKitchenObjetSoGameObjects)
        {
            kitchenObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObjectOnOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (var kitchenObject in listKitchenObjetSoGameObjects)
        {
            if (kitchenObject.kitchenObjectSo == e.kitchenObjectSo)
            {
                kitchenObject.gameObject.SetActive(true);
            }
        }
    }
}
