using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MultiplayerBuddyRoomFurnitureInit : MonoBehaviour {
    public static string FurnitureString;
    public List<int> furnitureSelected;
    public List<int> furnitureLevels;
    public List<BuildFurnitureNode> furnitureNodes;
    public BuildFurnitureRootNode rootFurnNode;
    public GameObject mainPoint;
    // Use this for initialization
    void Start() {
        if (PhotonNetwork.isMasterClient)
        {
            FurnitureString = PlayerPrefs.GetString("FurnitureString");
            Debug.Log("Output 1: " + FurnitureString);
            Debug.Log("Output 2: " + PlayerPrefs.GetString("FurnitureLevels"));

            string[] furnitureStringsplit = FurnitureString.Split(',');
            string[] furniturelvls = PlayerPrefs.GetString("FurnitureLevels").Split(',');

            Debug.Log("Length 1: " + furnitureStringsplit.Length);
            Debug.Log("Length 2: " + furniturelvls.Length);
            for (int i = 0; i < furnitureStringsplit.Length; i++)
            {

                furnitureSelected.Add(int.Parse(furnitureStringsplit[i]));

            }
            for (int i = 0; i < furniturelvls.Length; i++)
            {
                Debug.Log(furniturelvls[i]);
                furnitureLevels.Add(int.Parse(furniturelvls[i]));
            }
            for (int i = 0; i < 6; i++)//we should only ever be getting 6 items received
            {
                itemSelected[i] = PlayerPrefs.GetInt("Item"+(i+1),0);
            }
            UpdateMultiplayerFurniture();
        }
    }
    bool hasSet = false;
    int[] itemSelected = new int[]{ 0, 0, 0, 0, 0, 0 };
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Sending");
            stream.SendNext(PlayerPrefs.GetString("FurnitureString"));
            stream.SendNext(PlayerPrefs.GetString("FurnitureLevels"));
            stream.SendNext(PlayerPrefs.GetInt("Item1", 0));
            stream.SendNext(PlayerPrefs.GetInt("Item2", 0));
            stream.SendNext(PlayerPrefs.GetInt("Item3", 0));
            stream.SendNext(PlayerPrefs.GetInt("Item4", 0));
            stream.SendNext(PlayerPrefs.GetInt("Item5", 0));
            stream.SendNext(PlayerPrefs.GetInt("Item6", 0));
        }        
        else
        {
            if (!hasSet)
            {
                Debug.Log("Receiving");
                string furnitureStrings = (string)stream.ReceiveNext();
                string furnitureLevelss = (string)stream.ReceiveNext();
                Debug.Log(furnitureStrings);
                Debug.Log(furnitureLevelss);
                hasSet = true;
                FurnitureString = furnitureStrings;

                string[] furnitureStringsplit = furnitureStrings.Split(',');
                string[] furniturelvls = furnitureLevelss.Split(',');
                for (int i = 0; i < furnitureStringsplit.Length; i++)
                {
                    furnitureSelected.Add(int.Parse(furnitureStringsplit[i]));
                }
                for (int i = 0; i < furniturelvls.Length; i++)
                {
                    Debug.Log(furniturelvls[i]);
                    furnitureLevels.Add(int.Parse(furniturelvls[i]));
                }
                for (int i = 0; i < 6; i++)//we should only ever be getting 6 items received
                {
                    itemSelected[i] = (int)stream.ReceiveNext();
                    Debug.Log(itemSelected[i]);
                }
                UpdateMultiplayerFurniture();

            }

        }
    }
    public void UpdateMultiplayerFurniture()
    {//
        Debug.Log("Setting up the multiplayer furnitures");
        furnitureNodes = new List<BuildFurnitureNode>(GameObject.Find("BuildLocations").GetComponentsInChildren<BuildFurnitureNode>());
        Debug.Log(  furnitureNodes[0]                    );

        try { furnitureNodes[0].SelectNewFurniture(furnitureSelected[0], furnitureLevels[0]); } catch (System.Exception e) { Debug.LogError(e); }//, GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected)); } catch (System.Exception e) { }
        try { furnitureNodes[1].SelectNewFurniture(furnitureSelected[1], furnitureLevels[1]); } catch (System.Exception e) { Debug.LogError(e); }//, GameState.me.inventory.furnitureLevels.getFurnitureLevel(1, 1)); } catch (System.Exception e) { }
        try { furnitureNodes[2].SelectNewFurniture(furnitureSelected[2], furnitureLevels[2]); } catch (System.Exception e) { Debug.LogError(e); }//, GameState.me.inventory.furnitureLevels.getFurnitureLevel(2, 1)); } catch (System.Exception e) { }
        try { furnitureNodes[3].SelectNewFurniture(furnitureSelected[3], furnitureLevels[3]); } catch (System.Exception e) { Debug.LogError(e); }//, GameState.me.inventory.furnitureLevels.getFurnitureLevel(3, 1)); } catch (System.Exception e) { }
        try { furnitureNodes[4].SelectNewFurniture(itemSelected[0], 0); } catch (System.Exception e) { Debug.LogError(e); }//, 0); } catch (System.Exception e) { }
        try { furnitureNodes[5].SelectNewFurniture(itemSelected[1] , 0); } catch (System.Exception e) { Debug.LogError(e); }//, 0); } catch (System.Exception e) { }
        try { furnitureNodes[6].SelectNewFurniture(itemSelected[2] , 0); } catch (System.Exception e) { Debug.LogError(e); }//, 0); } catch (System.Exception e) { }
        try { furnitureNodes[7].SelectNewFurniture(itemSelected[3] , 0); } catch (System.Exception e) { Debug.LogError(e); }//, 0); } catch (System.Exception e) { }
        try { furnitureNodes[8].SelectNewFurniture(itemSelected[4] , 0); } catch (System.Exception e) { Debug.LogError(e); }//, 0); } catch (System.Exception e) { }
        try { furnitureNodes[9].SelectNewFurniture(itemSelected[5] , 0); } catch (System.Exception e) { Debug.LogError(e);}//, 0); } catch (System.Exception e) { }

    }
    // Update is called once per frame
    void Update () {
	
	}
}
