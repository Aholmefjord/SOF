using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkPetHomeSaveScript : Photon.MonoBehaviour {
	
	private static Dictionary<long, Furniture> furnitureItems; 
	
	//loading
	public void loadJSONStringToFurnitureData()
	{
		//Debug.Log ("try load fleet prefs " + playerPrefFleetName + " : " + PlayerPrefs.GetString(playerPrefFleetName,""));
		string savedPlayerFurnitureData = PlayerPrefs.GetString("FURNITUREHOMEDATA", "");
		
		if (savedPlayerFurnitureData == "") return;
		
		JSONObject obj = new JSONObject(savedPlayerFurnitureData);
		if (obj == JSONObject.nullJO) {
			return;
		}
		
		for (int j = 0; j < obj.Count; j++) {
			string furnitureName = obj[j]["id"].ToString();
			float posX = float.Parse(obj[j]["x"].ToString());
			float posY = float.Parse(obj[j]["y"].ToString());
			float posZ = float.Parse(obj[j]["z"].ToString());
			float rotY = float.Parse(obj[j]["rot_y"].ToString());
			
			Vector3 tVect = new Vector3(posX,posY,posZ);
			furnitureName = furnitureName.Replace("\"", "");
			string toLoadString = "PetHome/InventoryAssets/" + furnitureName;

#if UNITY_EDITOR
			Debug.Log("LOADED: " +toLoadString);
#endif
			GameObject justInstantiatedGO = PhotonNetwork.Instantiate(toLoadString, tVect, Quaternion.Euler(new Vector3(0,rotY,0)), 0) as GameObject;justInstantiatedGO.name = furnitureName;

			//photonView.RPC("setPropertiesOfNewGO ", PhotonTargets.All,toLoadString,tVect,furnitureName,rotY);
		}
	}


	/*
	[PunRPC]
	public void setPropertiesOfNewGO (string toLoadString, Vector3 tVect, string furnitureName,float rotY){

		justInstantiatedGO.GetComponent<Collider>().enabled=true;
			justInstantiatedGO.AddComponent<Furniture>();
			justInstantiatedGO.transform.SetParent(HomeNetworkManager.instance.editableGroup.transform,true);
			justInstantiatedGO.GetComponent<Furniture>().saveValues();
			
			HomeNetworkManager.instance.allInstantiatedEditables.Add(justInstantiatedGO);
		justInstantiatedGO.transform.localScale = new Vector3(Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE);
		Component tempAnimator = justInstantiatedGO.transform.Find("Mesh").GetComponent<Animator>();
		if(tempAnimator!=null){
			justInstantiatedGO.AddComponent<FurnitureNetworkAnimationController>();
			justInstantiatedGO.GetComponent<FurnitureNetworkAnimationController>().SetAnimator(tempAnimator as Animator);
			justInstantiatedGO.AddComponent<FurnitureNetwork>();
			justInstantiatedGO.GetComponent<PhotonView>().ObservedComponents.Add(justInstantiatedGO.GetComponent<FurnitureNetworkAnimationController>());
		}
	}*/
}
