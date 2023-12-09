using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ModelRotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public GameObject rotateObject;
	bool button_down;
	float start_position_x;
	float start_position_y;
	float start_rotation_y;
	float start_rotation_x;

	// Use this for initialization
	void Start () {
		button_down = false;
	}

	void Update() {
		if (!button_down) {
			//rotateObject.transform.Rotate (0, 0.1f, 0);
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		// Add event handler code here
		button_down = true;
		start_position_x = eventData.position.x;
		start_rotation_y = rotateObject.transform.eulerAngles.y;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		// Add event handler code here
		button_down = false;
	}

	public void OnDrag(PointerEventData data)
	{
		if (!button_down) {
			return;
		}

		// Rotate right left
		float moved_x = (start_position_x - data.position.x) / 5;
		Vector3 tmp = rotateObject.transform.eulerAngles;
		tmp.y = start_rotation_y + moved_x;

		rotateObject.transform.eulerAngles = tmp;
	}

}


















