using UnityEngine;
using System.Collections;

public class EnforceScale : MonoBehaviour {
	void LateUpdate () {
        transform.localScale = Vector3.one;
	}
}
