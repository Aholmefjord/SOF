using UnityEngine;
using System.Collections;

public class AvatarLoader : MonoBehaviour {
	public static AvatarLoader current;
	private int onLoadScale;
	private GameObject avatar;
	private bool divingMode;

	void Awake()
	{
		current = this;
	}

	public GameObject GetCustomAvatar(string avatar_path, string avatar_skin_path, bool divingMode = false)
	{
		this.onLoadScale = 1;
		return LoadCustomAvatarObject(avatar_path, avatar_skin_path, divingMode);
	}

	public GameObject GetAvatarDivingModeObject(int onLoadScale = 1)
	{
		this.onLoadScale = onLoadScale;
		divingMode = true;
		return LoadPlayerAvatarObject();
	}
	public GameObject GetAvatarObject(int onLoadScale = 1)
	{
        Debug.Log("Loading Avatar Object");
		this.onLoadScale = onLoadScale;
		divingMode = false;
		return LoadPlayerAvatarObject();
	}
    public GameObject GetAvatarObjectCutscene(int onLoadScale = 1)
    {
        
        Debug.Log("Loading Avatar Object");
        this.onLoadScale = onLoadScale;
        divingMode = false;
        GameObject avatar = LoadPlayerAvatarObject();
        avatar.transform.parent = transform;
        avatar.transform.localScale = new Vector3(1, 1, 1);
        avatar.transform.localPosition = new Vector3(0, 0, 0);
        avatar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        return avatar;
    }
    public string GetAvatarBasePath()
	{
		if (GameState.me == null ||
			GameState.me.avatar == null ||
			GameState.me.avatar.avatar == null ||
			GameState.me.avatar.avatar.prefab == null)
		{
			return "";
		}

		return Constants.AVATAR_BASE_PATH + GameState.me.avatar.avatar.prefab;
	}

	public string GetAvatarSkinPath()
	{
		if (GameState.me == null ||
			GameState.me.avatar == null //  ||
//			GameState.me.avatar.avatar == null ||
//			GameState.me.avatar.avatar.prefab == null
            )
		{
			return "";
		}

		return Constants.AVATAR_SKIN_BASE_PATH + GameState.me.avatar.avatarSkin.prefab; ;
	}

	private GameObject LoadPlayerAvatarObject()
	{
        //Debug.Log("Loading Avatar");
		if (GameState.me == null || 
			GameState.me.avatar == null 
//            ||
//			GameState.me.avatar.avatar == null 
 //           ||
//			GameState.me.avatar.avatar.prefab == null
            )
		{
			return null;
		}

		string avatar_path = GetAvatarBasePath();
        try
        {
            avatar = Instantiate(Resources.Load(avatar_path, typeof(GameObject))) as GameObject;
        }catch(System.Exception e)
        {
            MainNavigationController.GoToAvatarCreation();
        }
		avatar.transform.localPosition = new Vector3();
		//avatar.transform.localScale = new Vector3(0,0,0);
		AvatarInstanceController cont = avatar.AddComponent<AvatarInstanceController>();
		cont.divingMode = divingMode;
		cont.avatar = avatar;
		cont.avatarBaseString = avatar_path;
		cont.avatarSkinString = GetAvatarSkinPath();
		cont.loadMaterials = true;
		return avatar;
	}

	private GameObject LoadCustomAvatarObject(string avatar_path, string avatar_skin_path, bool divingMode = false)
	{
		avatar = Instantiate(Resources.Load(avatar_path, typeof(GameObject))) as GameObject;
		avatar.transform.localPosition = new Vector3();
		AvatarInstanceController cont = avatar.AddComponent<AvatarInstanceController>();
		cont.divingMode = divingMode;
		cont.avatar = avatar;
		cont.avatarBaseString = avatar_path;
		cont.avatarSkinString = avatar_skin_path;
		cont.loadMaterials = true;
		return avatar;
	}

    public static GameObject MakeAvatarObject(string avatar_path, string avatar_skin_path, bool divingMode = false)
    {
        GameObject avatar = Instantiate(Resources.Load(avatar_path, typeof(GameObject))) as GameObject;
        avatar.transform.localPosition = new Vector3();
        AvatarInstanceController cont = avatar.AddComponent<AvatarInstanceController>();
        cont.divingMode = divingMode;
        cont.avatar = avatar;
        cont.avatarBaseString = avatar_path;
        cont.avatarSkinString = avatar_skin_path;
        cont.loadMaterials = true;
        return avatar;
    }
}
