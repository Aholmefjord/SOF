using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler
{
	public bool Interactable;

	bool isPressed;

	//public Graphic TargetGraphic;

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Down");
	}

	void Start()
	{
		isPressed = false;
	}

	// Update is called once per frame
	void Update()
	{

	}
}
