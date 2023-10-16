using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiPlayerGamePaused;
    public event EventHandler OnMultiPlayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    private enum State{
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }
    //NetworkVariable是一种在客户端与服务端之间同步属性（变量）的方法
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private readonly NetworkVariable<float> countToStartTimer = new NetworkVariable<float>(3f);
    private readonly NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);    
    private readonly float gamePlayingTimerMax = 90f;
    private bool isLocalGamePaused;
    private readonly NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);  
    private bool isPlayerReady;
    //ulong为无符号long型，这里用来记录端口id
    private Dictionary<ulong, bool> playerReadyDictionary;  //记录多人玩家的准备情况
    private Dictionary<ulong, bool> playerPauseDictionary;  //记录多人玩家的暂停情况

    private bool autoTestGamePauseState;
    [SerializeField] private Transform playerPrefab;    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        //初始化
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {   
        //为状态改变时添加执行函数
        state.OnValueChanged += State_ValueChanged;
        isGamePaused.OnValueChanged += GamePaused_ValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
            //OnLoadEventCompleted在所有客户端均完成场景加载后调用执行
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
        }
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            //将生成的玩家作为对应端口的玩家，并设置随着场景加载而销毁
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePauseState = true;
    }

    private void GamePaused_ValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiPlayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiPlayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 状态改变时执行状态改变事件
    /// </summary>
    private void State_ValueChanged(State previousvalue, State newvalue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        InputManager.Instance.OnPauseAction += InputManagerOnPauseAction;
        InputManager.Instance.OnInteractionAction += InputManagerOnInteractionAction;
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        //状态转换只在服务端进行，由于state的值是进行同步的
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountDownToStart:
                countToStartTimer.Value -= Time.deltaTime;
                if (countToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
        // Debug.Log(state);
    }

    private void LateUpdate()
    {
        if (autoTestGamePauseState)
        {   //当有玩家断开时，连接的端口id并非立即更新，而是在下一帧更新，所以这里在进行测试时，也在下一帧才进行测试
            autoTestGamePauseState = false;
            TestGamePausedState();
        }
    }

    private void InputManagerOnInteractionAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isPlayerReady = true;   //修改为玩家准备
            //这里要先进行打开等待面板UI，再进行设置玩家准备，否则在最后一个准备后，其等待面板会未关闭
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
            // state = State.CountDownToStart;
            // OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    /// <summary>
    /// 多人游戏中设置玩家准备
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {   
        //serverRpcParams.Receive.SenderClientId为接入端口发送的端口id
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool ifAllPlayersReady = true;
        //检测所有玩家是否已经准备
        //NetworkManager.Singleton.ConnectedClientsIds为已连接的端口id
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {//有玩家还未准备
                ifAllPlayersReady = false;
                break;
            }
        }
        //所有玩家准备完毕，转换状态
        if (ifAllPlayersReady)
        {
            state.Value = State.CountDownToStart;
        }
    }

    private void InputManagerOnPauseAction(object sender, EventArgs e)
    {
        PauseGame();
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountDownStart()
    {
        return state.Value == State.CountDownToStart;  
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }
    public bool IsLocalPlayerReady()
    {
        return isPlayerReady;
    }
    public float GetCountDownStartTimer()
    {
        return countToStartTimer.Value;
    }

    public float GetGamePlayingNormalize()
    {
        return 1 - gamePlayingTimer.Value / gamePlayingTimerMax;
    }

    public void PauseGame()
    {
        Debug.Log(isGamePaused.Value);
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }    
    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
            {//有玩家暂停
                isGamePaused.Value = true;
                return;
            }       
        }
        //没有玩家暂停
        isGamePaused.Value = false;
    }
}
