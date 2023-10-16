using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private const String PLAYER_PREFS_BINDING = "InputBinding";
    public static InputManager Instance { get; private set; }
    
    public event EventHandler OnInteractionAction;//创建一个事件，以便于输入系统在按下按键后，执行该事件
    public event EventHandler OnInteractionAlternateAction;
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))   //加载之前更改后的键位
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDING));
        }
        playerInputActions.Enable();
        playerInputActions.Player.Interact.performed += Interaction;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate;
        playerInputActions.Player.pause.performed += Pause;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interaction;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate;
        playerInputActions.Player.pause.performed -= Pause;
        playerInputActions.Dispose();   //释放引用
    }

    private void Pause(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate(InputAction.CallbackContext obj)
    {
        OnInteractionAlternateAction?.Invoke(this,EventArgs.Empty);
    }

    private void Interaction(InputAction.CallbackContext obj)
    {
        // if (OnInteractionAction != null)
        // {
        //     OnInteractionAction(this,EventArgs.Empty);//this为当前对象，EventArgs当前事件的处理参数，empty为为该对象赋空值 
        // }
        OnInteractionAction?.Invoke(this,EventArgs.Empty);//该行执行功能与上面一致，当输入系统进行按键，并且该事件不为空时，执行该事件
    }
    public Vector2 GetMovementDir()
    {
        Vector2 moveDir = Vector2.zero;
        moveDir = playerInputActions.Player.Move.ReadValue<Vector2>();
        // if (Input.GetKey(KeyCode.W))
        // {
        //     moveDir.y = 1f;
        // }
        // if (Input.GetKey(KeyCode.S))
        // {
        //     moveDir.y = -1f;
        // }
        // if (Input.GetKey(KeyCode.A))
        // {
        //     moveDir.x = -1f;
        // }
        // if (Input.GetKey(KeyCode.D))
        // {
        //     moveDir.x = 1f;
        // }
        moveDir = moveDir.normalized;
        return moveDir;
    }

    public String GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            //ToDisplayString()只返回绑定的键值，而不需要其他的东西
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();           
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();            
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();           
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString(); 
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();           
            case Binding.Pause:
                return playerInputActions.Player.pause.bindings[0].ToDisplayString();            

        }
    }

    public void Rebinding(Binding binding,Action onActionRebound)
    {
        playerInputActions.Player.Disable();    //先关闭输入系统
        InputAction inputAction;
        int bindingIndex;
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.pause;
                bindingIndex = 0;
                break;

        }
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            playerInputActions.Enable();
            onActionRebound();  //调用传过来的函数
            PlayerPrefs.SetString(PLAYER_PREFS_BINDING,playerInputActions.SaveBindingOverridesAsJson());//保存已更改的键位
            PlayerPrefs.Save();
        }).Start();
    }
}
