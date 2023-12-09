using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class PowUnitSpawner : MonoBehaviour {
    public PowSpawnManager psm;
    public GameObject ObjectToSpawn;
    public bool isSelected; //used for the highlight, only changes color for now
    public bool canSpawn;
    public int selectedID;
    public List<GameObject> manaCosts;
    int ManaCost;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public Image frameImage;
    public Image fishImage;
    public Text newText;
    public Text OKText;
    void Start()
    {
        PowBaseCharacterStats pbcsTemp = new PowBaseCharacterStats(ObjectToSpawn.GetComponent<PowBaseCharacter>().FileToLoadStatsFrom);
        ManaCost = pbcsTemp.ManaCost;

        for(int i = 0; i < ManaCost; i++)
        {
            manaCosts[i].SetActive(true);
        }
        for(int i = ManaCost; i < manaCosts.Count; i++)
        {
            manaCosts[i].SetActive(false);
        }
    }

    public void Update()
    {

        canSpawn = PowGameController.Player.PlayerMana >= ManaCost;
        OKText.enabled = canSpawn;
        if (canSpawn)
        {
            frameImage.sprite = activeSprite;
        }else
        {
            frameImage.sprite = inactiveSprite;
        }
        fishImage.color = (canSpawn) ? Color.white : Color.gray;        
    }

	public void OnClick()
    {

        if(PowGameController.Player.PlayerMana >= ManaCost) {
            PowGameController.Player.RemoveMana(ManaCost);
            psm.SpawnOnClick(ObjectToSpawn);

            //SFX
            AudioSystem.PlaySFX("tako/SFX_Tako_Clickfish");
        }
    }
}
