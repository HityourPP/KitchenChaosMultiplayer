using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DelieverManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeComplete;    
    public event EventHandler OnRecipeSuccess; 
    public event EventHandler OnRecipeFailed;

    public static DelieverManager Instance { get; private set; }
    [SerializeField] private RecipeList_SO recipeListSo;
    private List<Recipe_SO> waitRecipeList;

    private float spawnerRecipeTimer;  //生成时间
    private readonly float spawnerRecipeTimerMax = 4f;  //生成时间
    private readonly int spawnerRecipeMaxCount = 4;  //生成最大数量
    private int successfulRecipesAmount = 0; //记录交付成功的数量

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        waitRecipeList = new List<Recipe_SO>();
        spawnerRecipeTimer = spawnerRecipeTimerMax;
        spawnerRecipeTimer = 3f;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnerRecipeTimer -= Time.deltaTime;
        if (spawnerRecipeTimer <= 0f)
        {
            spawnerRecipeTimer = spawnerRecipeTimerMax;
            if (GameManager.Instance.IsGamePlaying() && waitRecipeList.Count < spawnerRecipeMaxCount)
            {//当游戏开始时开始生成食谱
                int randomIndex = Random.Range(0, recipeListSo.recipeList.Count);
                SpawnNewWaitingRecipeClientRpc(randomIndex);
            }
        }
    }
    
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int index)  //注意必须以ClientRpc为结尾命名
    {
        Recipe_SO recipe = recipeListSo.recipeList[index];
        waitRecipeList.Add(recipe);  
        OnRecipeSpawn?.Invoke(this,EventArgs.Empty);    //生成食谱UI
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitRecipeList.Count; i++)
        {
            Recipe_SO recipe = waitRecipeList[i];
            if (recipe.kitchenObjectsSoList.Count == plateKitchenObject.GetKitchenObjectsList().Count)
            {//首先判断两个食谱的数量是否相同
                bool ifCanMatch = true;
                foreach (var recipeKithchenObjectSO in recipe.kitchenObjectsSoList)
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjects_SO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectsList())
                    {//遍历查看是否存在
                        if (plateKitchenObjectSO == recipeKithchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        ifCanMatch = false;
                        break;
                    }
                }
                if (ifCanMatch)
                {//匹配，进行交付
                    Debug.Log("交付");
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        //若未匹配，设置未匹配音效
        DeliveryInCorrectRecipeServerRpc();
    }
    /// <summary>
    /// 食谱未匹配时的操作
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryInCorrectRecipeServerRpc()  //在服务器端更新食谱，并广播给客户端
    {
        DeliveryInCorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliveryInCorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// 食谱匹配时的操作
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int index)  //在服务器端更新食谱，并广播给客户端
    {
        DeliveryCorrectRecipeClientRpc(index);
    }
    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int index)
    {
        waitRecipeList.RemoveAt(index); //将交付后的食谱移除
        successfulRecipesAmount++;
        OnRecipeComplete?.Invoke(this,EventArgs.Empty); //将已经完成的食谱UI删去
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<Recipe_SO> GetWaitRecipeList()
    {
        return waitRecipeList;
    }

    public int GetSuccessfulRecipeAmount()
    {
        return successfulRecipesAmount;
    }
}
