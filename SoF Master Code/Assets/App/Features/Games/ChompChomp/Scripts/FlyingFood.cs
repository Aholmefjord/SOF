using UnityEngine;
using System.Collections;

public class FlyingFood : MonoBehaviour {
	private Vector3 ownCharPlatePosition;
	private Vector3 friendCharPlatePosition;
	private Vector3 targetPlatePosition;
	private Vector3 startPosition;
	public float speedOfTransition = 10000.0f;
	public float timeTakenToEat = 3.0f;
	public bool ownPlayerEating;
	[HideInInspector] public MatchGameManager gm {get{return JMFUtils.gm;}}
	// Use this for initialization
	void Start () {

	}
	public void init(){
		startPosition = transform.position;
		ownCharPlatePosition = gm.ownCharPlate.transform.position;
		friendCharPlatePosition = gm.friendCharPlate.transform.position;

	}
	public void setOwnPlayerEating(bool ownPlayerEating){
		//to set friend or own 
		this.ownPlayerEating = ownPlayerEating;
		if (ownPlayerEating) {
			targetPlatePosition = ownCharPlatePosition;
			this.gameObject.tag = "OwnPlayerFood";
		} else {
			targetPlatePosition = friendCharPlatePosition;
			this.gameObject.tag = "FriendPlayerFood";
		}
		StartCoroutine (flyingMotionToPlate());
	}

	

	private IEnumerator flyingMotionToPlate(){
		while (true) {
			transform.position = Vector3.Lerp(transform.position,targetPlatePosition,Time.deltaTime*speedOfTransition);
			//Debug.Log(transform.position);
			
			if ((this.transform.position - targetPlatePosition).sqrMagnitude <= 1.0f) {
				break;
			}
			yield return null;
		}
		if (ownPlayerEating)
		{
			if (MatchGameController.current.ownPlayerAnimator != null)
			{
				MatchGameController.current.ownPlayerAnimator.CrossFade("Chew_State",0.1f);//Need eat state added first for new pet
			}
		} else {
			if (MatchGameController.current.friendAnimator != null)
			{
				MatchGameController.current.friendAnimator.Play("A_eat");
			}
		}
		StopAllCoroutines ();
		StartCoroutine (onThePlate ());
	}
	private IEnumerator onThePlate(){
		yield return new WaitForSeconds (timeTakenToEat);
		StopAllCoroutines();
		
		this.gameObject.tag = "ToBeDestroyedFood";

		if (ownPlayerEating && GameObject.FindGameObjectsWithTag("OwnPlayerFood").Length <= 0)
		{
			if (MatchGameController.current.ownPlayerAnimator != null)
			{
				MatchGameController.current.ownPlayerAnimator.CrossFade("Idle_State",0.1f); //Need eat state added first for new pet
			}
		}
		else if (!ownPlayerEating && GameObject.FindGameObjectsWithTag("FriendPlayerFood").Length <= 0) {
			if (MatchGameController.current.friendAnimator != null)
			{
				MatchGameController.current.friendAnimator.Play("A_idle");
			}
		}

		Destroy (this.gameObject);
	}
}
