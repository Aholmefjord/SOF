using UnityEngine;
using System.Collections;

public class GridPlaneScript : MonoBehaviour
{
	void Start()
	{
		GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(Mathf.RoundToInt(transform.localScale.x), Mathf.RoundToInt(transform.localScale.z));
	}
}
