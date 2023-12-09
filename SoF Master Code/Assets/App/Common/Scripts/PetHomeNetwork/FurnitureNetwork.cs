using UnityEngine;
using System.Collections;

public class FurnitureNetwork : Photon.MonoBehaviour {
	Animator furnitureAnim;
    static bool isNewHouse = true;
	bool checkSendFurnitureAnim = false;
	// Use this for initialization
	public GameObject misPlaceIndi;
	

	void Start(){
        try
        {
            misPlaceIndi = this.gameObject.transform.Find("MisPlace").gameObject;
            misPlaceIndi.SetActive(false);

        if (checkSendFurnitureAnim) transform.localScale = new Vector3(1, 1, 1);
        }catch(System.Exception e)
        {

        }

    }

	void Awake()
	{
	}
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		//this.photonView.RPC ("setPropertiesOfNewGO", PhotonTargets.All);
		//setPropertiesOfNewGO ();
		furnitureAnim = this.transform.Find("Mesh").GetComponent<Animator>();
		this.gameObject.GetComponent<Collider>().enabled=true;
		this.gameObject.transform.SetParent(HomeNetworkManager.instance.editableGroup.transform,true);
		HomeNetworkManager.instance.allInstantiatedEditables.Add(this.gameObject);
		this.gameObject.transform.localScale = new Vector3(Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE);
		Component tempAnimator = this.gameObject.transform.Find("Mesh").GetComponent<Animator>();


		if(tempAnimator!=null){
			furnitureAnim = this.transform.Find("Mesh").GetComponent<Animator>();
		}
	}
	/*
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//Debug.Log("pushing data");
			//We own this player: send the others our data
			// stream.SendNext((int)controllerScript._characterState);

			stream.SendNext(checkSendFurnitureAnim);
			if(checkSendFurnitureAnim){
				startAnimate();
			}
			//stream.SendNext(transform.position);
			//stream.SendNext(transform.rotation); 
		}
		else
		{

			//Network player, receive data
			// controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			checkSendFurnitureAnim = (bool)stream.ReceiveNext();
			if(checkSendFurnitureAnim){
				startAnimate();
			}
			//correctPlayerPos = (Vector3)stream.ReceiveNext();
			//correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
	*/
	void OnMouseDown() {
        try
        {
            this.photonView.RPC("startAnimate", PhotonTargets.All);
        }
        catch (System.Exception e)
        {

        }
		//checkSendFurnitureAnim=true;
	}
	[PunRPC]
	public void startAnimate()
	{
		//checkSendFurnitureAnim = false;
		if (furnitureAnim != null) {
			furnitureAnim.Play ("FurnitureAnimation");
		}
	}

}
