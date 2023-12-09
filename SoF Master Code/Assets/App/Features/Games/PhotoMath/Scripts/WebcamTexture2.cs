using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class WebcamTexture2 : MonoBehaviour {
    public RawImage rawimage;
    WebCamTexture webcamTexture;
    public RawImage rawimage2;
	static DeviceOrientation orientation;
    public AspectRatioFitter fit;
    public AspectRatioFitter fit2;
    Vector3 deviceCameraLocalScale;

    void Start()
    {
		deviceCameraLocalScale = this.GetComponent<RectTransform>().localScale;
		orientation = Input.deviceOrientation;
       // if (GameState.me == null)
       // {
       //     return;
       // }
            WebCamDevice[] devices = WebCamTexture.devices;
            bool foundDevice = false;
            for (var i = 0; i < devices.Length; i++)
            {
                if (devices[i].isFrontFacing)
                {
                    foundDevice = true;
                    webcamTexture = new WebCamTexture(devices[i].name, Screen.width,Screen.height);
//                    this.GetComponent<RectTransform>().localScale = new Vector3(-0.5f, 1, 1);

                }
            }
            if (!foundDevice)
            {
                webcamTexture = new WebCamTexture();
            }
            rawimage.texture = webcamTexture;
            rawimage.material.mainTexture = webcamTexture;
            rawimage2.texture = webcamTexture;
            rawimage2.material.mainTexture = webcamTexture;
            webcamTexture.Play();
    }

	void Update()
	{
        //if ((orientation != Input.deviceOrientation && Input.deviceOrientation == DeviceOrientation.LandscapeLeft) || (orientation != Input.deviceOrientation && Input.deviceOrientation == DeviceOrientation.LandscapeRight))
        //    if ((orientation != Input.deviceOrientation) && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight))
        //    {
        //        Debug.LogError("FLIP PLEASE");
        //       // deviceCameraLocalScale -= (2 * deviceCameraLocalScale);
        //        //this.gameObject.transform.localScale = deviceCameraLocalScale;
        //        orientation = Input.deviceOrientation;
        //    }

        //camera rotation code
        int orient = -webcamTexture.videoRotationAngle;
        rawimage.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        rawimage2.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        //anti stretch code
        float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
        fit.aspectRatio = ratio;
        fit2.aspectRatio = ratio;
        float scaleY;
        if (webcamTexture.videoVerticallyMirrored)
        {
            scaleY = -1.0f;
        }
        else
        {
            scaleY = 1.0f;
        }
        rawimage.rectTransform.localScale = new Vector3(1.0f, scaleY, 1.0f);
        
    }

    public void SwitchCamera()
    {
        try
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            webcamTexture.Stop();
            webcamTexture.deviceName = (webcamTexture.deviceName == devices[0].name) ? devices[1].name : devices[0].name;
            webcamTexture.Play();
        }
        catch(Exception e) { Debug.LogError("Unable to switch camera due to " + e); }
    }

	public void FlipCameraButton()//testing flipping of camera function
	{
		this.gameObject.transform.localScale = new Vector3(0.75f, -1, -1);
	}

	public void FlipCameraButton2()//testing flipping of camera function
	{
		this.gameObject.transform.localScale = new Vector3(-0.75f, 1, 1);
	}

    void OnDestroy()
    {
        Stop();
    }
	// Update is called once per frame
    public void Stop()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            Debug.Log("WebcamTexture is stopped");
        }
        else
        {
            Debug.Log("WebcamTexture is null");
        }
    }
}
/*
 public class PlayMovieTextureOnUI : MonoBehaviour 
 {
     public RawImage rawimage;
     void Start () 
     {
         WebCamTexture webcamTexture = new WebCamTexture();
         rawimage.texture = webcamTexture;
         rawimage.material.mainTexture = webcamTexture;
         webcamTexture.Play();
     }
 }*/