using UnityEngine;
using UnityEngine.UI;

public class QueueButtonScript : MonoBehaviour
{
	//public uint u_id;

	[SerializeField]
	GameObject type_sf;

	[SerializeField]
	Sprite[] typeSprite_sf;

	[SerializeField]
	Sprite[] alternateTypeSprit_sf;

	//[SerializeField]
	//GameObject indicator;

	[SerializeField]
	Sprite[] completed;

	public int i_cmd;

	public bool is_junctioned;

	private GameObject partS;

	void Start()
	{
		// Creation of the particle then destroy it 3 seconds later
		partS = Instantiate(Resources.Load("UIParticles")) as GameObject;
		partS.transform.SetParent(this.transform);
		partS.transform.localPosition = new Vector3(0, 0, 0);
		partS.transform.localRotation = new Quaternion(0, 0, 0, 0);
		Destroy(partS, 3.0f);
	}

	public void setIcon(int cmd, bool junctioned = false, int dir = 0)
	{
		if(!junctioned)
			type_sf.GetComponent<Image>().sprite = typeSprite_sf[cmd];

		else
			type_sf.GetComponent<Image>().sprite = alternateTypeSprit_sf[cmd];

		i_cmd = cmd;

		setDirection(dir);
	}

	public void setDirection(int dir)
	{
		Quaternion rotateDir = Quaternion.identity;

		switch (dir)
		{
			case KodableScript.UP:
				rotateDir = Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case KodableScript.DOWN:
				rotateDir = Quaternion.Euler(new Vector3(0, 0, 180));
				break;
			case KodableScript.LEFT:
				rotateDir = Quaternion.Euler(new Vector3(0, 0, 90));
				break;
			case KodableScript.RIGHT:
				rotateDir = Quaternion.Euler(new Vector3(0, 0, 270));
				break;
			default:
				break;
		}

		type_sf.GetComponent<RectTransform>().rotation = rotateDir;
		//indicator.GetComponent<RectTransform>().rotation = rotateDir;
		//indicator.SetActive(false);
    }

	

	public void buttonRemove()
	{
		//if(GameManagerScript.current.getState != GameStateEnum.Start)
		//return;
		if (KodableManagerScript.current.state == 1)
		{
			GameObject gameEssential = GameObject.Find("GameEssentials");
			if (gameEssential != null)
			{
				//Debug.Log("isfound");
				//Might need to change to a base game class for command queue based games
				gameEssential.GetComponent<KodableScript>().removeObjectQueue();
				Destroy(gameObject);
			}
		}
	}

	// If set to true, it will turn on a cross section, else, it will not take in the next command
	public void setJunctioned(bool enable)
	{
		is_junctioned = enable;
	}

	public void setIndicator(bool enable)
	{
		//indicator.SetActive(enable);
		type_sf.GetComponent<Image>().sprite = completed[i_cmd];
	}

	public void updateQueuePanel()
	{
		GameObject.Find("GameEssentials").GetComponent<KodableScript>().removeButtonsTrigger();
	}
}