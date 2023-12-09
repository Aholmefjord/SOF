using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowFriendlySpawn : PowSpawnManager
{
    public PowGameController controller;
    public PowSpawnUIManager manager;
    // Use this for initialization

    public void OnMouseDown()
    {
        Debug.Log("Clicked");
        if (manager.selectedUnit != null) {
            float amount = manager.selectedUnit.GetComponent<PowBaseCharacter>().GetManaCost();

            if (PowGameController.Player.RemoveMana(amount))
            {
                GOToSpawn.Add(manager.selectedUnit);
            }
            manager.SpawnObject();
        }
    }
}
