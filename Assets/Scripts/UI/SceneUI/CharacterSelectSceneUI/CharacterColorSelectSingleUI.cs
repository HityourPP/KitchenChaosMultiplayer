using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;                    //将自身的image赋值
    [SerializeField] private GameObject selectGameObject;    //将子对象selected赋值

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener((() =>
        {
            KitchenGameMultiPlayer.Instance.ChangePlayerColor(colorId);
        }));
    }

    private void Start()
    {
        image.color = KitchenGameMultiPlayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiPlayerOnPlayerDataNetworkListChanged;
    }

    private void OnDestroy()
    {
        KitchenGameMultiPlayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiPlayerOnPlayerDataNetworkListChanged;
    }

    private void KitchenGameMultiPlayerOnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (KitchenGameMultiPlayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectGameObject.SetActive(true);
        }
        else
        {
            selectGameObject.SetActive(false);
        }
    }
}
