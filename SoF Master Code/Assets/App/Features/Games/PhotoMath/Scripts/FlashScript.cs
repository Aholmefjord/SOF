using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AutumnInteractive.SimpleJSON;
using System.IO;
using UniRx;

public class FlashScript : MonoBehaviour {
    public float alphaMax = 0f;
    public RenderTexture extTexture;
    public Color white = Color.white;
    private int selfieCount;
    // Use this for initialization	
    private void Start()
    {
        if (!PhotoGallery.FindImageInGallery("BuddyPhoto"))
            PlayerPrefs.DeleteKey("BuddySelfieCount");

        selfieCount = PlayerPrefs.GetInt("BuddySelfieCount", 0);
    }
    // Update is called once per frame
    void Update() {
        if (alphaMax > 0)
        {
            alphaMax -= Time.deltaTime / 2;



        }
        if(white.a != alphaMax)
            white.a = alphaMax;
        try
        {
            if (GetComponent<RawImage>().color != white)
                GetComponent<RawImage>().color = white;
        }
        catch (Exception e) { }
    }
    public void CaptureScreenshot(bool showToast = false)
    {
        selfieCount++;
        PlayerPrefs.SetInt("BuddySelfieCount", selfieCount);
        alphaMax = 1;
        RenderTexture.active = extTexture;
        Texture2D selfie = new Texture2D(extTexture.width, extTexture.height, TextureFormat.RGB24, false);
        selfie.ReadPixels(new Rect(0, 0, extTexture.width, extTexture.height), 0, 0, false);
        selfie.Apply();
        byte[] bytes = selfie.EncodeToJPG(75);
        string filename = "BuddyPhoto" + selfieCount + ".jpg";


#if UNITY_EDITOR
		// HOWARD - DEPRECATED??? - TO EVALUATE
//        Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
//        string fileLocation2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\" + filename;
//        File.WriteAllBytes(fileLocation2, bytes);
//        if(JulesBox.HAS_FOUND_SERVER && JulesBox.USE_JULES_BOX_SOF_SERVER)
//        	UploadSelfie(bytes);
#else
        UploadSelfie(bytes);


#if UNITY_IPHONE

            string fileLocation2 = Path.Combine(GetiPhoneDocumentsPath(), filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
    //        Etcetera
            //if (Etcetera .saveImageToGallery(fileLocation2, filename)) ;
            EtceteraBinding.saveImageToPhotoAlbum(fileLocation2);
    //        GUIFriendsInRo
#elif UNITY_ANDROID


            string fileLocation2 = Path.Combine("/sdcard/Pictures", filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
            if (EtceteraAndroid.saveImageToGallery(fileLocation2, filename))
            {
                print("something");
            }
        if(showToast)
            EtceteraAndroid.showToast("Photo saved to device", false);
#endif
#endif
    }

    public void CaptureScreenshot(string fileName, int delay, bool showToast = false)
    {
        StartCoroutine(CaptureDelayedScreenshot(fileName, delay, showToast));
    }

    public IEnumerator CaptureDelayedScreenshot(string fileName, int delay, bool showToast)
    {
        yield return new WaitForSeconds(delay);

        string filename = fileName + ".jpg";
        alphaMax = 1;
        RenderTexture.active = extTexture;
        Texture2D screenshot = new Texture2D(extTexture.width, extTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, extTexture.width, extTexture.height), 0, 0, false);
        screenshot.Apply();
        byte[] bytes = screenshot.EncodeToJPG(75);

#if UNITY_EDITOR
        //Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
        string fileLocation2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\" + filename;
        File.WriteAllBytes(fileLocation2, bytes);
        //Debug.Log("Captured Screenshow");
#else

#if UNITY_IPHONE

            string fileLocation2 = Path.Combine(GetiPhoneDocumentsPath(), filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
    //        Etcetera
            //if (Etcetera .saveImageToGallery(fileLocation2, filename)) ;
            EtceteraBinding.saveImageToPhotoAlbum(fileLocation2);
    //        GUIFriendsInRo
#elif UNITY_ANDROID


            string fileLocation2 = Path.Combine("/sdcard/Pictures", filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
            if (EtceteraAndroid.saveImageToGallery(fileLocation2, filename))
            {
                print("something");
            }
        if(showToast)
            EtceteraAndroid.showToast("Photo saved to device", false);
#endif
#endif
    }

    public void CaptureScreenshot(string fileName, bool showToast = false)
    {
        string filename = fileName + ".jpg";
        alphaMax = 1;
        RenderTexture.active = extTexture;
        Texture2D screenshot = new Texture2D(extTexture.width, extTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, extTexture.width, extTexture.height), 0, 0, false);
        screenshot.Apply();
        byte[] bytes = screenshot.EncodeToJPG(75);

#if UNITY_EDITOR
        Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
        string fileLocation2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\" + filename;
        File.WriteAllBytes(fileLocation2, bytes);
        Debug.Log("Captured Screenshow");
#else

#if UNITY_IPHONE

            string fileLocation2 = Path.Combine(GetiPhoneDocumentsPath(), filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
    //        Etcetera
            //if (Etcetera .saveImageToGallery(fileLocation2, filename)) ;
            EtceteraBinding.saveImageToPhotoAlbum(fileLocation2);
    //        GUIFriendsInRo
#elif UNITY_ANDROID


            string fileLocation2 = Path.Combine("/sdcard/Pictures", filename);
            File.WriteAllBytes(fileLocation2, bytes);
            alphaMax = 1;
            if (EtceteraAndroid.saveImageToGallery(fileLocation2, filename))
            {
                print("something");
            }
        if(showToast)
            EtceteraAndroid.showToast("Photo saved to device", false);
#endif
#endif
    }

    public void UploadSelfie(byte[] selfie)
    {
		// HOWARD - Deprecated - TO REPLACE
//        WWWForm form = new WWWForm();        
//        form.AddField("token", "0h1clU0x59937H6rM3BRTG6q1V2u7qhs");
//        form.AddField("userName", GameState.me.id);
//        form.AddBinaryData("selfieData", selfie, "selfie.jpg", "image/jpg");
//
//        AppServer.CreateImagePost("upload", form)
//        .Subscribe(
//            x => UploadActivity(x), // onSuccess
//            ex => AppServer.ErrorResponse(ex, "Unable to upload selfie") // onError
//        ); 
    }
    public void UploadActivity(string location) {
        Debug.Log(location);
        JSONNode node = JSONNode.Parse<JSONNode>(location);
        if(node["file_location"] != null)        
        GameState.PostSelfie(3, Constants.IMAGE_URL +"selfie/" + node["file_location"]);
        Debug.Log("Posting to: " + Constants.IMAGE_URL + "selfie/" + node["file_location"]);
    }
    public static string GetiPhoneDocumentsPath()
    {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
        // Application.dataPath returns              
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
        // Strip "/Data" from path 
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        // Strip application name 
        path = path.Substring(0, path.LastIndexOf('/'));
        return path + "/Documents";
    }
}
