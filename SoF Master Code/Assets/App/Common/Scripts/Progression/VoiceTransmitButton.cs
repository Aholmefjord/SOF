using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ExitGames.Demos.DemoPunVoice;
    public class VoiceTransmitButton : MonoBehaviour {
    public Image image;
    public PushToTalkScript ptts;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        image.color = (ptts.rec.IsTransmitting)? Color.green: Color.red;
	}
    public void Transmit(bool on)
    {
        ptts.rec.Transmit = on;
    }

}
