using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PhotoMathQuit : MonoBehaviour
{
    public WebcamTexture2 texture;
    public RawImage texture2;

    public RawImage texture3;
    public RawImage texture4;
    public void Start()
    {
        texture4.gameObject.SetActive(false);
    }



    // Update is called once per frame
    public void ExitSelfie()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        texture2.texture = texture4.texture;
        texture2.material.mainTexture = null;
        texture3.texture = texture4.texture;
        texture3.material.mainTexture = null;
        
#else
        try
        {
            if (texture != null)
            {
                texture.Stop();
            }
            else
            {
                Debug.Log("ExitSelfie texture is null");
            }

            if(GameSys.StateManager.Instance.NumberOfStates() > 0)
            { 
                GameSys.IState frontState = GameSys.StateManager.Instance.GetFirstState();

                if (frontState.Name.Equals("Camera State"))
                {
                    frontState.IsDone = true;
                    Debug.Log("ExitSelfie set camera state done");
                }
            }
        }
        catch (Exception e) { Debug.LogError(e); }
#endif

        MainNavigationController.GotoMainMenu();
    }
}
