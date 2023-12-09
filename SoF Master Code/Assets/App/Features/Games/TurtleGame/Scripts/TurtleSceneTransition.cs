using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurtleSceneTransition : MonoBehaviour {

    public string SceneName;

	void Start () {
	}
    // Update is called once per frame
    void Update() {
    }

    IEnumerator LoadScene()
    {
        if (SceneName == "")
            yield break;

        AsyncOperation async;
        async = SceneManager.LoadSceneAsync(SceneName);  // Activate Loading Of Scene
        //async.allowSceneActivation = false;             // Not To Activate The Scene Immediately
        Debug.Log("Loading");

        yield return async;
    }

    public void ChangeScene() // To Activate Change Scene
    {
        StartCoroutine(LoadScene());
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
