using UnityEngine;
using System.Collections;

public class RewardClass : MonoBehaviour
{
    public string code;
    public int JewelAmount;
    public string furnitureUnlock;
    public bool claimed;
    public string game_id;
    public virtual void claim()
    {
    }
}
