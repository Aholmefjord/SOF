using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UndwaterCaustic : MonoBehaviour {

	public float fps = 30.0f;
	public Texture2D[] frames;

	private int frameIndex;
	private Projector projector;
    public bool UseImageRaw;
    //public Material mat;
	void Start () {
		//projector = GetComponent<Projector>();
		NextFrame();
		InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
	}
	
	void NextFrame () {
        if (UseImageRaw)
            GetComponent<RawImage>().texture = frames[frameIndex];
        else
            GetComponent<Renderer>().material.SetTexture("_MainTex", frames[frameIndex]);
		frameIndex = (frameIndex + 1) % frames.Length;
	}
}
