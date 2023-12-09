using UnityEngine;
using System.Collections;

public class MapRipples : MonoBehaviour {

    [SerializeField]
    RectTransform[] Ripples;

    [SerializeField]
    Vector2[] StartEndPos;

    [SerializeField]
    float speed;

	// Use this for initialization
	void Start () {
        foreach (RectTransform i in Ripples)
        {
            Vector2 tmpVec = i.anchoredPosition;
            tmpVec.y = StartEndPos[0].y;
            i.anchoredPosition = tmpVec;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    foreach(RectTransform i in Ripples)
        {
            Vector2 tmpVec = i.anchoredPosition;
            tmpVec.x = tmpVec.x + speed * Time.deltaTime;
            i.anchoredPosition = tmpVec;

            if(StartEndPos[1].x < i.anchoredPosition.x)
            {
                i.anchoredPosition = StartEndPos[0];
            }
        }
	}
}
