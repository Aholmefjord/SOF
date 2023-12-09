using UnityEngine;
using System.Collections;

public class Map_Fish_Movement : MonoBehaviour {

    [SerializeField]
    float FishSpeed = 1.0f;

    [SerializeField]
    bool Left = true;
	
	// Update is called once per frame
	void Update () {

        Vector3 tmp = gameObject.transform.position;

        // Spawns left, moves right
        if (Left)
        {
            tmp.x += FishSpeed * Time.deltaTime;
            gameObject.transform.position = tmp;
        }

        else
        {
            tmp.x -= FishSpeed * Time.deltaTime;
            gameObject.transform.position = tmp;
        }

        //print(gameObject.transform.position);
    }

    public void SetDirection(bool left)
    {
        Left = left;
    }
}
