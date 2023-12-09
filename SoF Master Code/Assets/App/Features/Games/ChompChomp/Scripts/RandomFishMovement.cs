using UnityEngine;
using System.Collections;

public class RandomFishMovement : MonoBehaviour {

	public float xMinVal;
	public float xMaxVal;
	public float yMinVal;
	public float yMaxVal;
	public float minStopTime;
	public float maxStopTime;
	private Vector3 destinationPoint;
	private Vector3 frontFacingScale;
	private Vector3 backFacingScale;

	// Use this for initialization
	void Start () {
		frontFacingScale = this.transform.localScale;
		backFacingScale = this.transform.localScale;
		backFacingScale.x = -frontFacingScale.x;
		StartCoroutine (fishStartMove ());
	
	}
	
	private IEnumerator fishStartMove(){
		destinationPoint = new Vector3 (Random.Range (xMinVal, xMaxVal), Random.Range (yMinVal, yMaxVal), this.transform.position.z);
		if ((this.transform.position.x - destinationPoint.x) > 0) {
			this.transform.localScale = backFacingScale;
		} else {
			this.transform.localScale = frontFacingScale;
		}
		while (Vector3.Distance(this.transform.position,destinationPoint)>1.0f) {
			this.transform.position = Vector3.Slerp(this.transform.position,destinationPoint,Time.fixedDeltaTime);
			yield return null;
		}
		StartCoroutine (fishStopMove ());
	}
	private IEnumerator fishStopMove(){
		yield return new WaitForSeconds (Random.Range (minStopTime, maxStopTime));
		StartCoroutine (fishStartMove ());
	}
}
