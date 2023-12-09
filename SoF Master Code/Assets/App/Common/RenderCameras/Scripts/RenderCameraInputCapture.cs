using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class RenderCameraInputCapture : MonoBehaviour {
    public Camera renderCam;
    public RenderTexture texture;
    public Camera UICam;
    public bool LowQuality;
   // public float CanvasWidth;
//    public float CanvasHeight;
  //  public float CameraWidth;
 //   public float CameraHeight;
	// Use this for initialization
	void Start () {
//        renderCam.targetTexture.Release();
        renderCam = Camera.main;
        int width = Screen.width;
        int height = Screen.height;
        if (LowQuality)
        {
            width = Screen.width / 2;
            height = Screen.height /2;
        }
      //  Screen.SetResolution(width, height, true);
        //texture = new RenderTexture(width, height, 16);
        //renderCam.targetTexture = texture;
        //GetComponent<RawImage>().texture = texture;
	}

    void DebugPoint(PointerEventData ped)
    {

    }
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            Vector2 localCursor = Input.mousePosition;
            Camera.main.ScreenPointToRay(Input.mousePosition);//UICam
            RaycastHit hitInfo; //Stores the information about the hit

            if (Physics.Raycast(Camera.main.ScreenPointToRay(localCursor), out hitInfo))
            {
                AbstractClickable clickable = hitInfo.collider.gameObject.GetComponent<AbstractClickable>();
                if (clickable != null)
                {
                    Debug.Log("Hit an object");
                    clickable.Click();
                }
            }
            Debug.Log("LocalCursor:" + localCursor);
        }
	}
}
