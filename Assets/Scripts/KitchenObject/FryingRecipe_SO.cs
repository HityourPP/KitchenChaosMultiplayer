using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class FryingRecipe_SO : ScriptableObject
{
    public KitchenObjects_SO input;
    public KitchenObjects_SO output;
    public float fryingTimerMax;
}
