using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PowSpawnUIManager : MonoBehaviour {
    public GameObject selectedUnit;
    public PowSpawnManager psm;
    public List<PowUnitSpawner> spawners;
    public Text newText;
    int selectedItem;
    public void Start()
    {
        spawners = new List<PowUnitSpawner>(transform.GetComponentsInChildren<PowUnitSpawner>());
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].isSelected = false;
            spawners[i].selectedID = i;
        }
    }
    
    public void SelectObject(int index)
    {
        if(selectedUnit != null)
        spawners[selectedItem].isSelected = false;

        if (spawners[index].canSpawn)
        {
            spawners[index].isSelected = true;
            selectedUnit = spawners[index].ObjectToSpawn;
            selectedItem = index;
        }
    }
    public void SpawnObject()
    {
        spawners[selectedItem].isSelected = false;
        spawners[selectedItem].ObjectToSpawn.GetComponent<PowBaseCharacter>().Start();
        selectedItem = -1;
        selectedUnit = null;

    }
}
