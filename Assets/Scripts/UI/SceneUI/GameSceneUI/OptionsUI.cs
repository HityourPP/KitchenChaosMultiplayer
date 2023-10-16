using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;   
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlterButton;
    [SerializeField] private Button pauseButton;
    
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlterText;
    [SerializeField] private TextMeshProUGUI pauseText;

    [SerializeField] private Transform pressAnyKeyToRebind;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        
        soundEffectButton.onClick.AddListener((() =>
        {
            SoundManager.Instance.ChangeVolume(); 
            UpdateVirtual();
        }));
        musicButton.onClick.AddListener((() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVirtual();
        }));
        closeButton.onClick.AddListener((() =>
        {
            Hide();
        }));
        moveUpButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Move_Up); }));
        moveDownButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Move_Down); }));
        moveLeftButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Move_Left); }));
        moveRightButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Move_Right); }));
        interactButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Interact); }));
        interactAlterButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.InteractAlternate); }));
        pauseButton.onClick.AddListener((() => { Rebinding(InputManager.Binding.Pause); }));
    }

    private void Start()
    {
        GameManager.Instance.OnLocalGameUnPaused += LocalGameManagerOnLocalGameUnPaused;//游戏未中断时也要将该窗口关闭
            UpdateVirtual();
        Hide();
        HidePressAnyKeyToRebindTransform(); 
    }

    private void LocalGameManagerOnLocalGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVirtual()
    {//Mathf.Round()四舍五入
        soundEffectText.text = "Sound Effect:" + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music:" + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        moveUpText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Move_Up);
        moveDownText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Move_Down);
        moveLeftText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Move_Left);
        moveRightText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Move_Right);
        interactText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Interact);
        interactAlterText.text = InputManager.Instance.GetBindingText(InputManager.Binding.InteractAlternate);
        pauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Pause);
    }
    public void Show()
    {   
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressAnyKeyToRebindTransform()
    {
        pressAnyKeyToRebind.gameObject.SetActive(true);
    }    
    private void HidePressAnyKeyToRebindTransform()
    {
        pressAnyKeyToRebind.gameObject.SetActive(false);
    }

    private void Rebinding(InputManager.Binding binding)
    {
        ShowPressAnyKeyToRebindTransform();
        InputManager.Instance.Rebinding(binding,()=>
        {
            HidePressAnyKeyToRebindTransform();
            UpdateVirtual();
        });
    }
}
