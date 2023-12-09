using UnityEngine;
using System.Collections;

public class CameraButton : MonoBehaviour {
    public FlashScript ext;
    public PhotoGallery ext2;
	// Use this for initialization

    public void OnClick()
    {
        ext.CaptureScreenshot();
        ext2.UpdateGallery();
    }
}
