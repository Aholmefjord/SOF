using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AvatarInstanceController : MonoBehaviour {
	//Used to load materials etc once mesh is loaded
	public string avatarBaseString;
	public string avatarSkinString;
	public GameObject avatar;
	public float onLoadScale = 1;
	public bool divingMode = false;
	public bool loadMaterials = true;
    public PhotonView photonView;
	void Start () {
        
		avatar = gameObject;
        
        if (loadMaterials)
		{
            //Debug.Log("Do Load Materials");
			//Do a check to see if need to auto load materials. Delayed version is for multiplayer purpose.
			LoadAvatarMaterials();
		}
    }

	public void LoadAvatarMaterials()
	{
		if (SceneManager.GetActiveScene().name == "mapNew" ||
			SceneManager.GetActiveScene().name == "Scenes/mapNew" ||
			SceneManager.GetActiveScene().name == "Freeze_scene" ||
			SceneManager.GetActiveScene().name == "Scenes/Freeze_scene")
		{
			onLoadScale = 1.5f;
		}
		if (SceneManager.GetActiveScene().name == "Scenes/mapNewMulti" || SceneManager.GetActiveScene().name == "mapNewMulti")
			onLoadScale = 15f;
		if (transform.parent != null && (SceneManager.GetActiveScene().name == "Scenes/mapNewMulti" || SceneManager.GetActiveScene().name == "mapNewMulti"))
			transform.parent.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			//Debug.Log("Really Loaded Materials: " + this.gameObject.name);
		if (onLoadScale != 1)
		{
			avatar.transform.localScale = new Vector3(onLoadScale, onLoadScale, onLoadScale);
		}
        if (SceneManager.GetActiveScene().name == "Scenes/mapNewMulti" || SceneManager.GetActiveScene().name == "mapNewMulti")
        {
            avatar.transform.position = new Vector3(avatar.transform.position.x, avatar.transform.position.y, avatar.transform.position.z);
        }

        Material body_mat = (Material)Resources.Load(avatarSkinString, typeof(Material));

		string petfacebodypath = avatarSkinString + "_face";
		Material face_mat = (Material)Resources.Load(petfacebodypath, typeof(Material));

		Transform petAvatarBase = avatar.transform;
        
		if (petAvatarBase == null)
		{
			Debug.LogWarning("petAvatarBase not found");
			return;
		}
        if (PhotonNetwork.isMasterClient)
        {

        }else
        {
          //  avatar.transform.parent.gameObject.GetComponent<WalkingAvatar>().enabled = false;
            //avatar.transform.parent.gameObject.GetComponent<AIPathFinder>().enabled = false;
        }
        Transform head = petAvatarBase.Find("Mesh_Head");
		if (head != null)
		{

			ReplaceMaterials(head.gameObject, body_mat, face_mat);

			if (divingMode)
			{
				Transform headwearHolder = petAvatarBase.Find("Rig_Master/Biped/Biped Pelvis/Biped Spine/Biped Spine1/Biped Neck/Biped Head/Headwear_Holder");
				//Add helmet
				GameObject helm = Instantiate(Resources.Load("avatar/head/head_diving_helmet", typeof(GameObject))) as GameObject;
				helm.transform.SetParent(headwearHolder, false);
			}
		}
		Transform tail = petAvatarBase.Find("Mesh_Tail");
		if (tail != null)
		{
			ReplaceMaterials(tail.gameObject, body_mat);
		}
		Transform body = petAvatarBase.Find("Mesh_Body");
		if (body != null)
		{
			if (divingMode)
			{
				string avatar_skin_diving_suit_path = Constants.AVATAR_SKIN_BASE_PATH + "diving_mat_blue";
				Material body_diving_suit_mat = (Material)Resources.Load(avatar_skin_diving_suit_path, typeof(Material));
				ReplaceMaterials(body.gameObject, body_diving_suit_mat);
			}
			else
			{
				ReplaceMaterials(body.gameObject, body_mat);
			}
		}
        petAvatarBase.SetAsFirstSibling();
	}

	private void ReplaceMaterials(GameObject gameObject, params Material[] newMat)
	{
		Renderer r = gameObject.GetComponent<Renderer>();
		Material[] materials = gameObject.GetComponent<Renderer>().materials;
		materials[0] = newMat[0];
		if (materials.Length > 1 && newMat[1] != null)
		{
			materials[1] = newMat[1];
		}
		r.materials = materials;
	}

}
