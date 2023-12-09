using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurtleGameLogic : MonoBehaviour {

    public Text GameTimer;
    public float SetTimer; //Set in Seconds

    private float TimerHolder; //Seconds Counter

    private float MinuteHolder; //Minute Counter
    private float SecondHolder;

    public TurtleSceneTransition scene;
    [SerializeField]
    private TurtleGameScore player;

	// Use this for initialization
	void Start () {
        TimerHolder = Mathf.Max(0, SetTimer);
        player = GameObject.FindGameObjectWithTag("TurtleDontDestroyOnLoad").GetComponent<TurtleGameScore>();
    }
	
	// Update is called once per frame
	void Update () {
        TimerHolder -= Time.deltaTime;
        MinuteHolder = Mathf.FloorToInt(TimerHolder / 60);
        SecondHolder = Mathf.FloorToInt(TimerHolder - (MinuteHolder * 60));

        if (TimerHolder <= 0.0f)
        {
            Debug.Log("Dead");
            scene.ChangeScene();
        }
        else
        {
            //GameTimer.text = (((TimerHolder)).ToString("F1") + "s");
            GameTimer.text = string.Format("{0:0}:{1:00}", MinuteHolder, SecondHolder);
        }

        if (player.GetScore() < 0)
        {
            Debug.Log("Dead");
            scene.ChangeScene();
        }

    }
}
