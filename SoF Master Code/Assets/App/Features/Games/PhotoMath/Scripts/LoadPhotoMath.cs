using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadPhotoMath : MonoBehaviour {
    public string name;
    public void OnClick()
    {///
       // Application.LoadLevel(name);
//        SceneManager.LoadScene(name,LoadSceneMode.Single);
        MainNavigationController.GoToScene("buddySelfie");
    }
}
