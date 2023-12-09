using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NetworkCameraControlGameMode : MonoBehaviour {
	#if UNITY_EDITOR
	public float dragSpeed = 0.3f;
	private Vector3 dragOrigin;
	#endif
	public float PanSpeed = 0.025F;
	public float PinchSpeed = 0.05F;
	private bool enablePanZoom = false;
	private float MinZoom = 0.7f;
	private float MaxZoom = 2.2f;
	
	// Update is called once per frame, of course.
	void Update () {
		/*
		if (HomeManager.instance && HomeManager.instance.getBuildMode())
		{
			return; //Cannot pan/zoom in build mode
		}
*/
		if (enablePanZoom) {
			// Check if we have two fingers down.
			
			if (Input.touchCount == 2 && !EventSystem.current.IsPointerOverGameObject() && HomeManager.instance.getBuildMode())
			{
				Touch touch1 = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				
				// Find out how the touches have moved relative to eachother.
				Vector2 curDist = touch1.position - touch2.position;
				Vector2 prevDist = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);
				
				float touchDelta = curDist.magnitude - prevDist.magnitude;
				
				// Translate along local coordinate space.
				//Camera.main.transform.Translate (0, 0, touchDelta * PinchSpeed);   //For perspective camera
				
				Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - touchDelta * PinchSpeed, MinZoom, MaxZoom); //For Ortho camera
				return; //Dont allow pan while zooming
			}
			
			// Check if we have one finger down, and if it's moved.
			// You may modify this first portion to '== 1', to only allow pinching or panning at one time.
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject() && HomeManager.instance.getBuildMode()) {
				Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
				Camera.main.transform.position -= Camera.main.transform.right * touchDeltaPosition.x * PanSpeed;
				Camera.main.transform.position -= Camera.main.transform.up * touchDeltaPosition.y * PanSpeed;
				
				HomeManager.instance.disableEditableControlButtons();
			}
			
			
			
			#if UNITY_EDITOR
			
			if (Input.GetMouseButton(0)&& !EventSystem.current.IsPointerOverGameObject())
			{
				Debug.Log("MOVING CAMERA");
				Camera.main.transform.position -= Camera.main.transform.right * Input.GetAxis("Mouse X") * PanSpeed * 10;
				Camera.main.transform.position -= Camera.main.transform.up * Input.GetAxis("Mouse Y") * PanSpeed * 10;

			}
			
			//if(move.sqrMagnitude>0){
			//	GameManager.instance.disableEditableControlButtons();
			//}
			#endif
			
		}
	}
	public void enablePanAndZoom(bool toEnable){
		enablePanZoom = toEnable;
	}
}