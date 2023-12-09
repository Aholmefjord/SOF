using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour {

    public List<Transform> Children = new List<Transform>();
    public GameObject[] Lane;
    public float Speed;
    [SerializeField]
    private int CurrentLaneNo;
    [SerializeField]
    private bool KeyPressed;

    // Use this for initialization
    void Start () {
        CurrentLaneNo = 1;
        KeyPressed = false;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Translate(new Vector3(Time.fixedDeltaTime, 0.51098f * Time.fixedDeltaTime, 0));
        Debug.Log(CurrentLaneNo);
        Debug.Log(KeyPressed);

        foreach (Transform child in Children)
        {
            if (child.position != Lane[CurrentLaneNo].transform.position)
            {
                child.position = Vector3.MoveTowards(child.position, Lane[CurrentLaneNo].transform.position, Speed);
                KeyPressed = false;
            }
            else
                KeyPressed = false;
        }

    }

    public void RightClick()
    {
        if (CurrentLaneNo < 2 && KeyPressed == false)
        {
            CurrentLaneNo += 1;
            KeyPressed = true;
        }
    }

    public void LeftClick()
    {
        if (CurrentLaneNo > 0 && KeyPressed == false)
        {
            CurrentLaneNo -= 1;
            KeyPressed = true;
        }
    }
}
