using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtlePlatformDeGenerator : MonoBehaviour {

    public Transform point;

    // Use this for initialization
    void Start()
    {
        point = GameObject.FindGameObjectWithTag("DestroyPlatform").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < point.position.x)
        {
            Destroy(gameObject);
        }
    }
}
