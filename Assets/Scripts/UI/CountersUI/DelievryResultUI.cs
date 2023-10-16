using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DelievryResultUI : MonoBehaviour
{
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        DelieverManager.Instance.OnRecipeComplete += DeliveryManagerOnRecipeComplete;
        DelieverManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManagerOnRecipeFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        anim.SetTrigger("PopUp");
        backGroundImage.color = failColor;
        iconImage.sprite = failSprite;
        messageText.text = "Delievry\nFailed";

    }

    private void DeliveryManagerOnRecipeComplete(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        anim.SetTrigger("PopUp");
        backGroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messageText.text = "Delievry\nSuccess";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
