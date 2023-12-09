using UnityEngine;
using System.Collections;

public class NetworkAnimationController : Photon.MonoBehaviour
{
	[SerializeField]
	CharacterController ctrler;
	[SerializeField]
	Animator anim;
	
	CharacterMotor charMotor;
	//to track velocity as character controller velocity will sometimes return 0
	float lastVel;
	
	void Start()
	{
		//set upper body layer weight
		//anim.SetLayerWeight(1, 1f);
//		charMotor = GetComponent<CharacterMotor>();
	}
	
	void Update()
	{

	}
	
	public void startAttack()
	{
		anim.CrossFade("Attack", 0.2f, 1);
	}

	public void SetAnimator(Animator anim)
	{
		this.anim = anim;
	}
}