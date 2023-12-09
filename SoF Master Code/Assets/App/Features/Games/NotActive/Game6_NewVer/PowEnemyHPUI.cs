using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowEnemyHPUI : MonoBehaviour {
    public float ratio;
    //float zeroPoint = 155;
	// Use this for initialization
	void Start () {
        //-145

    }

    // Update is called once per frame
    void Update () {
        RectTransform r = this.GetComponent<RectTransform>();
        Vector3 newScale = new Vector3(ratio, 1, 1);

        this.GetComponent<RectTransform>().localScale = newScale;
            //float newLeft = -155 + (zeroPoint * ratio);
        //this.GetComponent<RectTransform>().offsetMax = new Vector2(newLeft, r.offsetMax.y);
        //        this.GetComponent<RectTransform>().position = // new Vector3(newLeft, r.position.y, 0);
        //        this.GetComponent<RectTransform>().sizeDelta = new Vector3(newLeft, r.sizeDelta.y);
    }
}
