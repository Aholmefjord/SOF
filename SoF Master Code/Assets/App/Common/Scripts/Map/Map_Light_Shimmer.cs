using UnityEngine;
using System.Collections;

public class Map_Light_Shimmer : MonoBehaviour {

    [SerializeField]
    UnityEngine.UI.Image currObj;

    [SerializeField]
    float shimmerSpeed = 0.5f;

    private float trackAlpha = 0.0f;
    private bool shimmerUp = false;
    private Color col;

	// Use this for initialization
	void Start () {
        trackAlpha = currObj.color.a;
        col = currObj.color;

        if (trackAlpha < 0.5f)
            shimmerUp = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if(shimmerUp)
        {
            col = currObj.color;
            col.a = currObj.color.a + (shimmerSpeed * Time.deltaTime);
            currObj.color = col;

            if (col.a >= 0.95f)
                shimmerUp = false;
        }

        else
        {
            col = currObj.color;
            col.a = currObj.color.a - (shimmerSpeed * Time.deltaTime);
            currObj.color = col;

            if (col.a < 0.0f)
                shimmerUp = true;
        }
	}
}
