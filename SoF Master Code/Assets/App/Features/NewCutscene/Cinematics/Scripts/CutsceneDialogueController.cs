using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;
using DG.Tweening;

public class CutsceneDialogueController : MonoBehaviour {
    const string CinematicsAudioLocation = "cinematics-audio-bundle";   // TOM: temp solution

    public static bool isHomeCinematic = false;
    public static bool isPartTwo = false;
    public static bool needsBuddy = true;
    public GameObject otherGameObjectToDisableWhenActive;
    public AudioSource dialogueAudioSource;
    public AudioSource effectsAudioSource;
    public AudioSource bgmAudioSource;
    public List<AudioClip> audioClipsToPlay;
    public List<AudioClip> soundEffects;
    public List<string> textToDisplay;
    public List<string> nameToDisplay;
    public List<int> dialogueTypes;
    public Text characterDisplayText;
    public Text characterNameDisplayText;
    public string descriptorFileLocation;
    public string descriptorFileLocation2;
    public float bgmVolume;
    GameObject bgmObject;
    public GameObject ConfirmObject;
    public GameObject ContinueObject;
    public GameObject nextSceneObject;
    public GameObject buddyObject;
    Tweener t;


    private TextAsset descriptorFile;
    private TextAsset descriptorFile_2;

    public bool IsAddingModule;
    public UnityEngine.EventSystems.EventSystem es;
    public void EnableIQTest(int i)
    {
        GameObject b = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ui/IQTest"));
        b.GetComponent<IQTest>().finishCinematicObject = this.gameObject;
        b.transform.parent = this.transform.parent;
        b.GetComponent<IQTest>().Start(i);

    }
    // Use this for initialization
    private void Awake()
    {
        Debug.Log(Application.loadedLevelName);
        if (Application.loadedLevelName == "mapNew") {
            //Debug.Log("Was equal");
            if (!isHomeCinematic)
            {
                GetComponent<Animator>().enabled = false;
            }
            else
            {
                    EndGamePanel.showCutscene = true;
                    GetComponent<Animator>().enabled = true;
            }
        }
    }
    void Start () {
        if (!Application.loadedLevelName.Equals("Cinematics_chpt1"))
        {
            needsBuddy = true;
        }
        GameObject g = GameObject.Find("JULES_LOADING_02");
        if(g != null)
        {
            Destroy(g);
        }
        if (IsAddingModule)
        {
            es = GetComponent<UnityEngine.EventSystems.EventSystem>();
            es.enabled = false;
            es.enabled = true;
        }

        GetComponent<Animator>().SetBool("IsScenePartTwo", isPartTwo);
        try
        {
            if (needsBuddy)
            {
                Destroy(buddyObject.transform.GetChild(0).gameObject);
                GameObject avatar = AvatarLoader.current.GetAvatarObjectCutscene();
                avatar.transform.parent = buddyObject.transform;
            }
        }catch(System.Exception)
        {

        }
    //    for (int i = 0; i < GetComponent<Animator>().layerCount-1; i++) {
  //          GetComponent<Animator>().SetLayerWeight(i, 1);
//        }

        if (dialogueAudioSource == null) dialogueAudioSource = GetComponent<AudioSource>();
        if (effectsAudioSource == null) effectsAudioSource = GetComponent<AudioSource>();
        if (bgmAudioSource == null) bgmAudioSource = GetComponent<AudioSource>();

        if(otherGameObjectToDisableWhenActive != null)
        {
            otherGameObjectToDisableWhenActive.SetActive(false);
        }
        bgmObject = GameObject.Find("Audio_BGM");
        descriptorFile = (TextAsset)Resources.Load(MultiLanguage.getInstance().getCinematicScriptsLocation() + descriptorFileLocation, typeof(TextAsset));
        descriptorFile_2 = (TextAsset)Resources.Load(MultiLanguage.getInstance().getCinematicScriptsLocation() + descriptorFileLocation2, typeof(TextAsset));
        //Debug.Log(MultiLanguage.getInstance().getCinematicScriptsLocation() + descriptorFileLocation);
        //Debug.Log(MultiLanguage.getInstance().getCinematicScriptsLocation() + descriptorFileLocation2);
        JSONClass c = (!isPartTwo)?JSONClass.Parse<JSONClass>(descriptorFile.text): JSONClass.Parse<JSONClass>(descriptorFile_2.text);

        AssetBundle bundle = null;
        JULESTech.Resources.AssetBundleManager.LoadAssetBundle(CinematicsAudioLocation, (returnBundle) => {
            bundle = returnBundle;
            if (bundle == null) {
                // error happened;
                //yield break;
                return;
            }
            try {
                foreach (JSONNode t in c["Dialogue"].AsArray) {
                    textToDisplay.Add(t["text"].Value);
                    try {
                        //audioClipsToPlay.Add(Resources.Load<AudioClip>(t["audio"].Value));
                        audioClipsToPlay.Add(bundle.LoadAsset<AudioClip>(t["audio"].Value));
                    } catch (System.Exception) {
                        audioClipsToPlay.Add(bundle.LoadAsset<AudioClip>("null"));
                    }
                    try {
                        dialogueTypes.Add(t["dialogType"].AsInt);

                    } catch (System.Exception) {

                    }
                    try {
                        //Debug.Log(t["Speaker"].ToString() + ":" + t["Speaker"].ToString().Equals("\"Buddy\""));
                        if (t["Speaker"].ToString().Equals("\"Buddy\"")) {
                            try {
                                nameToDisplay.Add(GameState.playerBuddy.name + ":");
                            } catch (System.Exception) {
                                nameToDisplay.Add(MultiLanguage.getInstance().getString("cinematics_Buddy") + ":");
                            }
                        } else {
                            nameToDisplay.Add(MultiLanguage.getInstance().getString("cinematics_" + t["Speaker"]) + ":");
                        }
                    } catch (System.Exception) {
                        nameToDisplay.Add("");
                    }
                }
                characterDisplayText.text = "";
                ContinueObject.SetActive(false);
                ConfirmObject.SetActive(false);
                foreach (JSONNode t in c["soundEffects"].AsArray) {
                    //soundEffects.Add(Resources.Load<AudioClip>(t.Value));
                    soundEffects.Add(bundle.LoadAsset<AudioClip>(t.Value));
                }
            } catch (System.Exception) {

            }
        });
        
    }
    bool tried = false;
    public float volumeBackground = 1f;
    // Update is called once per frame
    void Update () {
        /*
        if (!tried)
            try
        {

            bgmObject.GetComponent<AudioSource>().volume = bgmVolume;
        }catch(System.Exception e)
        {
                tried = true;
        }
        try
        {
            bgmAudioSource.volume = volumeBackground;
        }
        catch(System.Exception e)
        {

        }
        */
    }
    #region AnimatingCharacters
    public void GoToChompChompIntro()
    {
        CutsceneDialogueController.isPartTwo = false;
        MainNavigationController.GoToCinematic("Cinematics_chpt3");
    }
    public void TriggerTransition()
    {
        HideDialogBox();
        GetComponent<Animator>().SetTrigger("Transition");
    }
    public void TriggerConfirm()
    {

        GetComponent<Animator>().SetTrigger("Confirm");
    }
    public void TriggerCharacterHappy()
    {
        GetComponent<Animator>().SetTrigger("HappyCharacter");
    }
    public void TriggerCharacterSpecial()
    {
        GetComponent<Animator>().SetTrigger("SpecialCharacter");
    } 
    public void TriggerCharacterSad()
    {
        GetComponent<Animator>().SetTrigger("SadCharacter");
    }
    public void TriggerCharacterAngry()
    {
        GetComponent<Animator>().SetTrigger("AngryCharacter");
    }
    public void TriggerCharacterNeutral()
    {
        GetComponent<Animator>().SetTrigger("NeutralCharacter");
    }
    public void TriggerNo() {
        GetComponent<Animator>().SetTrigger("No");
    }
    public void EnterSally()
    {
        GetComponent<Animator>().SetTrigger("EnterSally");
    }
    public void LeaveSally()
    {
        GetComponent<Animator>().SetTrigger("LeaveSally");
    }
    public void EnterDoodle()
    {
        GetComponent<Animator>().SetTrigger("EnterDoodle");
    }
    public void LeaveDoodle()
    {
        GetComponent<Animator>().SetTrigger("LeaveDoodle");
    }
    public void EnterBuddy()
    {
        GetComponent<Animator>().SetTrigger("EnterBuddy");
    }
    public void LeaveBuddy()
    {
        GetComponent<Animator>().SetTrigger("LeaveBuddy");
    }
    public void ShowEgg()
    {
        GetComponent<Animator>().SetTrigger("ShowEgg");
    }
    public void HideEgg()
    {
        GetComponent<Animator>().SetTrigger("HideEgg");
    }
    public void TriggerContinue()
    {
        t.Kill();
        characterDisplayText.text = "";
        try { 
        characterNameDisplayText.text = "";
        }catch(System.Exception)
        {

        }
        GetComponent<Animator>().SetTrigger("Continue");
    }
    public void TriggerCustom(string s)
    {
        GetComponent<Animator>().SetTrigger(s);
    }
    public void TriggerNextScene()
    {
        GetComponent<Animator>().SetTrigger("NextScene");
    }
    int panelType = 0;
    public void SetPanelType(int type)
    {
        panelType = type;
    }
    #endregion
    #region animation triggers
    public void SetSpeechText(int i)
    {
        Debug.Log("Setting Speech Text to index: " + i);
        characterDisplayText.text = "";
        if(dialogueTypes.Count > 0){//incase people didn't set this in JSON
            SetPanelType(dialogueTypes[i]);
        }
        try {
            t = characterDisplayText.DOText(textToDisplay[i], audioClipsToPlay[i].length, true, ScrambleMode.None, null).OnComplete<Tweener>(new TweenCallback(SpeechComplete));
            dialogueAudioSource.Stop();
        dialogueAudioSource.PlayOneShot(audioClipsToPlay[i]);
        }catch(System.Exception)
        {
           t = characterDisplayText.DOText(textToDisplay[i], 1, true, ScrambleMode.None, null).OnComplete<Tweener>(new TweenCallback(SpeechComplete));
        }
        try
        {
            characterNameDisplayText.text = nameToDisplay[i];
        }catch(System.Exception)
        {
            characterNameDisplayText.text = "";
        }
    }
    public void PlaySoundEffects(int i)
    {

    }
    public void SpeechComplete()
    {
        switch (panelType) {
            case 0:
                ContinueObject.SetActive(true);
                break;
            case 1:
                ConfirmObject.SetActive(true);
                break;
            case 2:
                break;
            case -1:
                break;
            case -2:
                characterDisplayText.text = "";
                break;
            default:
                ContinueObject.SetActive(true);
                break;              
        }
    }
    public void DisableFinishScene()
    {
        try
        {
            nextSceneObject.SetActive(true);
        }
        catch(System.Exception)
        {

        }
        otherGameObjectToDisableWhenActive.SetActive(true);
        gameObject.SetActive(false);

    }
    public AudioClip s;
    public void PlayAudioSound(string sound)
    {
        AssetBundle bundle = null;
        // this works only because AssetBundle has already been loaded
        JULESTech.Resources.AssetBundleManager.LoadAssetBundle(CinematicsAudioLocation, (result) => { bundle = result; });
       // effectsAudioSource.Stop();
        Debug.Log(sound);
        //s = Resources.Load(sound) as AudioClip;
        s = bundle.LoadAsset<AudioClip>(sound);
        effectsAudioSource.volume = 100f;

        if (s != null)
        {
            effectsAudioSource.PlayOneShot(s);
            //bgmAudioSource.PlayOneShot(s);
        }
        else
        {
            //s = Resources.Load("null") as AudioClip;
            s = bundle.LoadAsset<AudioClip>("null");
            s = null;
            effectsAudioSource.PlayOneShot(s);
            //bgmAudioSource.PlayOneShot(s);
            Debug.Log(sound + " was Null");

        }
    }
    public void PlayAudioBGM(string sound) {
        AudioSystem.PlayBGMCutscene(sound, true);
    }
    public void ClearDialog()
    {
        Debug.Log("Clearing Dialog");
        characterNameDisplayText.text = "";
        characterDisplayText.text = "";
    }
    public void ShowDialogBox()
    {
        Debug.Log("Showing Dialog");
        GetComponent<Animator>().SetTrigger("ShowDialog");
    }
    public void HideDialogBox()
    {
        Debug.Log("Hiding Dialog");
        GetComponent<Animator>().SetTrigger("HideDialog");
    }
    public void ReturnToHome(int i)
    {
        switch (i) {
            case 0:
                MainNavigationController.GoToScene("mapNew");
                break;
            case 1:
                MainNavigationController.GoToScene("start_menu");
                break;
            case 2:
                PlayerPrefs.SetInt("NeedsLogout", 1);
                MainNavigationController.GoToScene("start_menu");
                break;
        }
    }
    public void TriggerBuddyState(string state)
    {
        switch (state)
        {
            case "happy":
                buddyObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("cheer");
                break;
            case "sad":

                break;
            case "excited":
                buddyObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("cheer");
                break;
        }
    }
    public void ReturnToStartMenu(bool logout)
    {
        MainNavigationController.GoToStartMenu(true);
    }
    public void GoToScene(string scene)
    {
       
       if(scene== "pearly_main_screen")
        {
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "pearly";
            gameState.startLevel = 1;
            gameState.endLevel = 100;

            GameSys.StateManager.Instance.AddFront(gameState);
        }
       else if (scene == "crab_main_screen")
        {
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "crab";
            gameState.startLevel = 1;
            gameState.endLevel = 100;

            GameSys.StateManager.Instance.AddFront(gameState);
        }
        else if (scene == "Tangram Main Menu")
        {
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "manta";
            gameState.startLevel = 1;
            gameState.endLevel = 100;

            GameSys.StateManager.Instance.AddFront(gameState);
        }
        else if (scene == "TsumTsum Main Screen")
        {
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "tumble";
            gameState.startLevel = 1;
            gameState.endLevel = 100;

            GameSys.StateManager.Instance.AddFront(gameState);
        }
        else if (scene == "tako_level_select")
        {
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "tako";
            gameState.startLevel = 1;
            gameState.endLevel = 10;

            GameSys.StateManager.Instance.AddFront(gameState);
        }
        else
             
        {
            Debug.Log("scenename" + scene);
            MainNavigationController.GoToScene(scene);
        }

        


    }
    public void FinishBuddySignup()
    {
        GameObject.Find("CreationController").GetComponent<CreationController>().FinishPetName();
    }
    public void Logout()
    {
        PlayerPrefs.DeleteKey("jules_access_token");
        MainNavigationController.GoToScene("start_menu");
    }
    public void TriggerEgg()
    {

        GetComponent<Animator>().SetTrigger("Egg");
    }
    public void TriggerCutsceneThree()
    {
        isHomeCinematic = true;
        MainNavigationController.GoToScene("mapNew");
    }

    public void PlayBackgroundMusic(string file)
    {
        AssetBundle bundle = null;
        JULESTech.Resources.AssetBundleManager.LoadAssetBundle(CinematicsAudioLocation, (result) => { bundle = result; });    // this works only because AssetBundle has already been loaded
        try { 
        if (bgmObject != null)
            bgmObject.GetComponent<AudioSource>().Stop();
        }catch(System.Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log("Playing Background cutscene audio");
        bgmAudioSource.clip = Resources.Load<AudioClip>(file);
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
        bgmAudioSource.volume = 1f;
        //bgmAudioSource.PlayOneShot(Resources.Load<AudioClip>(file));
    }
    public void StopBackgroundMusic()
    {
        bgmAudioSource.loop = false;
        bgmAudioSource.Stop();
    }
    #endregion
}
