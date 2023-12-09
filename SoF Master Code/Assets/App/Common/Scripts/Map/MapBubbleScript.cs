using UnityEngine;
using System.Collections;

public class MapBubbleScript : MonoBehaviour {

    [SerializeField]
    Vector3 UpperLimit;

    [SerializeField]
    Vector3 ResetLocation;

    [SerializeField]
    float Speed;

    [SerializeField]
    Vector3 RespawnSpot;

	// Use this for initialization
	void Start () {
#if UNITY_IOS
		gameObject.SetActive(false);
#endif
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 tmp = this.transform.position;
        tmp.y = tmp.y + Speed * Time.deltaTime;
        this.transform.position = tmp;

        Color tmpCol = this.GetComponent<UnityEngine.UI.Image>().color;
        tmpCol.a -= Speed * Time.deltaTime * 0.0008f;
        this.GetComponent<UnityEngine.UI.Image>().color = tmpCol;

        if (this.transform.position.y >= UpperLimit.y)
        {
            this.transform.position = RespawnSpot;
            Color col = this.GetComponent<UnityEngine.UI.Image>().color;
            col.a = 1.0f;
            this.GetComponent<UnityEngine.UI.Image>().color = col;
        }
	}
}
