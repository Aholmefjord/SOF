using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnswerButtons : MonoBehaviour {
	public int iPosition;
	Image i;
	[SerializeField]
	Color c;
	Worksheet ws;

	void Start()
	{
		i = this.gameObject.GetComponent<Image> ();
		ws = GameObject.Find ("Worksheet").GetComponent<Worksheet> ();
	}

	public void TellWorksheetRegisterSelected()
	{
		ws.dmdDel(iPosition);
		//ws.RegisterSelected (iPosition);
	}

	void ChangeisSelectedTo(bool bBool)
	{
		if (bBool == true)
		{
			i.color = c;
		}
		else
		{
			i.color = Color.white;
		}
	}
}
