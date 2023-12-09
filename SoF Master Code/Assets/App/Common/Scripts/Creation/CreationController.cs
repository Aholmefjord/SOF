using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UniRx;
using System;
using System.Linq;
using JULESTech;

public class CreationController : MonoBehaviour {
    public CutsceneDialogueController cdc;
    public static CreationController current;
    public JSONClass petAssetsList;
    public GameObject petHolder;
    public Animator petAnimator;

    public Button confirmButton;

    public GameObject enterNamePanel;
    public InputField usernameText;

    public Text petNameText;
    public Text bodyNameText;
    public Text eyeNameText;

    private int currentAvatarIndex;
    private int currentBodyIndex;
    private int currentEyeIndex;

    [SerializeField]
    Text DescriptionField;

    [Space(16)]
    public GameObject errorPopup;
    public bool isCinematic;
    void Awake()
    {
        current = this;
        LoadPetJSONData();
		try
		{
			if (GameState.playerBuddy != null)
			{
				if (GameState.playerBuddy.name.Length > 0)
				{
					usernameText.text = GameState.playerBuddy.name;
					usernameText.interactable = false;
				}
			}
		}
		catch (Exception e)
		{

		}
        if (!isCinematic)
        {
            if (GameState.me.avatar == null)
            {
                currentAvatarIndex = 1;
                currentBodyIndex = 0;
                currentEyeIndex = 0;
            }
            else
            {
                currentAvatarIndex = GameState.me.avatar.avatarId;
                currentBodyIndex = GameState.me.avatar.skinId;
                currentEyeIndex = GameState.me.avatar.eyeId;
            }
        }else
        {
            currentAvatarIndex = 1;
            currentBodyIndex = 0;
            currentEyeIndex = 0;
        }
        //SelectNextPet();
        enterNamePanel.SetActive(true);
    }

    void Start()
    {
        SetupUI();
        LoadPet();
        AudioSystem.PlayBGM("bgm_creation");
    }
    
    public void SetupUI()
    {
        MultiLanguageApplicator langProc = new MultiLanguageApplicator(GameObject.Find("UICanvas"));
        langProc.ApplyText("BannerImage/Text"                   , "create_pet_title");
        langProc.ApplyText("Name Panel/Text"                    , "create_pet_name");
        langProc.ApplyText("Name Panel/Placeholder"             , "create_pet_name_place_holder");
        langProc.ApplyText("Pet Base Panel/Pet Name Text"       , "create_pet_pet_type");
        langProc.ApplyText("Body Group Panel/Body Value Text"   , "create_pet_body_type");
        langProc.ApplyText("Description Panel/Text"             , "create_pet_description");
        langProc.ApplyText("Error Panel/Panel/Text"             , "create_pet_error_text");
        langProc.ApplyText("Error Panel/Button/Text"            , "create_pet_error_button");
    }

    void LoadPetJSONData()
    {
        TextAsset t = (TextAsset)Resources.Load("data/pets", typeof(TextAsset));
        petAssetsList = JSONNode.Parse(t.text) as JSONClass;
    }

    public void SelectNextPet()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (currentAvatarIndex >= petAssetsList["pets"].Count - 1)
        {
            currentAvatarIndex = 0;
        }
        else
        {
            currentAvatarIndex++;
        }

        currentBodyIndex = 0;
        LoadPet();

        DescriptionField.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"] + "_" + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"] + "_desc");
        //DescriptionField.text = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["desc"];
    }

    public void SelectPrevPet()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (currentAvatarIndex <= 0)
        {
            currentAvatarIndex = petAssetsList["pets"].Count - 1;
        }
        else
        {
            currentAvatarIndex--;
        }

        //PlayPetSFX(petAssetsList["pets"][currentAvatarIndex]["name"]);

        currentBodyIndex = 0;
        LoadPet();

        DescriptionField.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"] + "_" + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"] + "_desc");
        //DescriptionField.text = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["desc"];
    }

    public void LoadPet(int index)
    {
        // Destroy old pet objects if any
        foreach (Transform g in petHolder.GetComponentsInChildren<Transform>())
        {
            if (g.gameObject != petHolder)
            {
                Destroy(g.gameObject);
            }
        }

        // Get resource pathname
        string petpath = "avatar/prefab/" + petAssetsList["pets"][index]["prefab"];
        Debug.Log(petpath);
        // Load new pet
        GameObject go = Instantiate(Resources.Load(petpath, typeof(GameObject))) as GameObject;
        go.transform.localPosition = new Vector3();
        go.transform.SetParent(petHolder.transform, false);
        go.name = "pet_mesh";

        petAnimator = go.GetComponent<Animator>();
    }

    void LoadPet()
    {
        LoadPet(currentAvatarIndex);
        petNameText.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"]);
        Invoke("LoadPetBody", 0.0f);
    }

    public void SelectNextBody()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (currentBodyIndex >= petAssetsList["pets"][currentAvatarIndex]["body"].Count - 1)
        {
            currentBodyIndex = 0;
        }
        else
        {
            currentBodyIndex++;
        }
        LoadPetBody();

        DescriptionField.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"] + "_" + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"] + "_desc");
        //DescriptionField.text = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["desc"];
    }

    public void SelectPrevBody()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (currentBodyIndex <= 0)
        {
            currentBodyIndex = petAssetsList["pets"][currentAvatarIndex]["body"].Count - 1;
        }
        else
        {
            currentBodyIndex--;
        }
        LoadPetBody();

        DescriptionField.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"] + "_" + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"] + "_desc");
        //DescriptionField.text = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["desc"];
    }

    void LoadPetBody()
    {
        bodyNameText.text = MultiLanguage.getInstance().getString("create_pet_" + petAssetsList["pets"][currentAvatarIndex]["name"] + "_" + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"]);
        LoadPetBody(currentAvatarIndex, currentBodyIndex, currentEyeIndex);
    }

    public void LoadPetBody(int petIndex, int bodyIndex, int eyeIndex = -1)
    {
        string petbodypath = "avatar/skin/" +
            petAssetsList["pets"][petIndex]["body"][bodyIndex]["prefab"];

        Material body_mat = (Material)Resources.Load(petbodypath, typeof(Material));
        Debug.Log("Load Body: " + petIndex + ", " + bodyIndex + ", " + petbodypath + ": " + body_mat);

        string petfacebodypath = "avatar/skin/" +
            petAssetsList["pets"][petIndex]["body"][bodyIndex]["prefab"] + "_face";
        Debug.Log("Load Bodyface: " + petIndex + ", " + bodyIndex + ", " + petfacebodypath);

        Material face_mat = (Material)Resources.Load(petfacebodypath, typeof(Material));

        Transform petAvatarBase = petHolder.transform.GetChild(0);
        if (petAvatarBase == null)
        {
            Debug.LogWarning("petAvatarBase not found");
            return;
        }
        Transform head = petAvatarBase.Find("Mesh_Head");
        if (head != null)
        {
            ReplaceMaterials(head.gameObject, body_mat, face_mat);
        }
        Transform tail = petAvatarBase.Find("Mesh_Tail");
        if (tail != null)
        {
            ReplaceMaterials(tail.gameObject, body_mat, face_mat);
        }
        Transform body = petAvatarBase.Find("Mesh_Body");
        if (body != null)
        {
            ReplaceMaterials(body.gameObject, body_mat, face_mat);
        }

    }

    private void ReplaceMaterials(GameObject gameObject, params Material[] newMat)
    {
        Debug.Log("ReplaceMaterials " + gameObject.name);
        Renderer r = gameObject.GetComponent<Renderer>();
        Material[] materials = gameObject.GetComponent<Renderer>().materials;
        materials[0] = newMat[0];
        if (materials.Length > 1 && newMat[1] != null)
        {
            materials[1] = newMat[1];
        }
        r.materials = materials;

        /*if (eyeIndex != -1)
		{
			// Try to load eye texture as well. Otherwise separate calls to load eye at the same time will fail.
			string path = "avatar/skin/" + petAssetsList["eyes"][eyeIndex]["file"];
			Texture tex = Resources.Load(path) as Texture;
			face_mat.SetTexture("_BlendTex", tex);
			Debug.Log("Load Eye: " + eyeIndex + ", " + path + ", " + tex.name);
		}*/
    }
    public void ConfirmPetCreationClickCinematic()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (!isPassLocalVulgarityCheck())
        {
            DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_username_not_allowed"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
            return;
        }
        SubmitPetCreationCutscene();
    }
    public void ConfirmPetCreationClick()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_click");

        if (!isPassLocalVulgarityCheck())
        {
            DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_username_already_in_use"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
            return;
        }

        if (usernameText.text != "" && usernameText.text.Replace(@"\s+", "") != "")
        {
            SubmitPetCreation();
        }
        else
        {
            // Show an error
            errorPopup.SetActive(true);
        }
    }

     bool isPassLocalVulgarityCheck()
    {
        //local vulgarity check
        string[] vulgarArr = new string[]{ "a_s_s", "c0ck", "eat a dick", "ham flap", "n1gga", "s hit", "4r5ea2m", "c0cksucker", "eat hair pie", "hardcoresex", "n1gger", "s.o.b.",
            "50 yard cunt punta55", "carpet muncher", "ejaculate", "hell", "nazi", "s_h_i_t", "5h1tamateur", "carpetmuncher", "ejaculated", "heshe", "need the dick",
            "sadism", "5hitanal", "cawk", "ejaculates", "hoar", "nigg3r", "sadist", "anal impaler", "chink", "ejaculating", "hoare", "nigg4h", "sandbar", "anal leakage",
            "choade", "ejaculatings", "hoer", "nigga", "sausage queen", "anilingus", "chota bags", "ejaculation", "homo", "niggah", "schlong", "anus", "cipa",
            "ejakulate", "homoerotic", "niggas", "screwing", "ar5e", "cl1t", "erotic", "hore", "niggaz", "scroat", "arrse", "clit", "f u c k", "horniest", "nigger",
            "scrote", "arse", "clit licker", "f u c k e r", "horny", "niggers", "scrotum", "arsehole", "clitoris", "f_u_c_k", "hotsex", "nob", "semen", "ass", "clits",
            "f4nny", "jackoff", "nob jokey", "sex", "ass fuck", "clitty litter", "facial", "jack-off", "nobhead", "sh!+", "asses", "clusterfuck", "fag", "jap", "nobjocky",
            "sh!t", "assfucker", "cnut", "fagging", "jerk", "nobjokey", "sh1t", "ass-fucker", "cock", "faggitt", "jerk-off", "numbnuts", "shag", "assfukka", "cock pocket",
            "faggot", "jism", "nut butter", "shagger", "asshole", "cock snot", "faggs", "jiz", "nutsack", "shaggin", "asshole", "cockface", "fagot", "jizm", "omg", "shagging",
            "assholes", "cockhead", "fagots", "jizz", "orgasim", "shemale", "assmucus", "cockmunch", "fags", "kawk", "orgasims", "shi+", "assmunch", "cockmuncher", "fanny",
            "kinky Jesus", "orgasm", "shit", "asswhole", "cocks", "fannyflaps", "knob", "orgasms", "shit fucker", "autoerotic", "cocksuck", "fannyfucker", "knob end", "p0rn",
            "shitdick", "b!tch", "cocksucked", "fanyy", "knobead", "pawn", "shite", "b00bs", "cocksucker", "fatass", "knobed", "pecker", "shited", "b17ch", "cock-sucker",
            "fcuk", "knobend", "penis", "shitey", "b1tch", "cocksucking", "fcuker", "knobend", "penisfucker", "shitfuck", "ballbag", "cocksucks", "fcuking", "knobhead", "phonesex",
            "shitfull", "ballsack", "cocksuka", "feck", "knobjocky", "phuck", "shithead", "bang (one's) box", "cocksukka", "fecker", "knobjokey", "phuk", "shiting", "bangbros",
            "cok", "felching", "kock", "phuked", "shitings", "bareback", "cokmuncher", "fellate", "kondum", "phuking", "shits", "bastard", "coksucka", "fellatio", "kondums",
            "phukked", "shitted", "beastial", "coon", "fingerfuck", "kum", "phukking", "shitter", "beastiality", "cop some wood", "fingerfucked", "kummer", "phuks", "shitters",
            "beef curtain", "cornhole", "fingerfucker", "kumming", "phuq", "shitting", "bellend", "corp whore", "fingerfuckers", "kums", "pigfucker", "shittings", "bestial",
            "cox", "fingerfucking", "kunilingus", "pimpis", "shitty", "bestiality", "cum", "fingerfucks", "kwif", "piss", "skank", "bi+ch", "cum chugger", "fist fuck", "l3i+ch",
            "pissed", "slope", "biatch", "cum dumpster", "fistfuck", "l3itch", "pisser", "slut", "bimbos", "cum freak", "fistfucked", "labia", "pissers", "slut bucket", "birdlock",
            "cum guzzler", "fistfucker", "LEN", "pisses", "sluts", "bitch", "cumdump", "fistfuckers", "lmao", "pissflaps", "smegma", "bitch tit", "cummer", "fistfucking", "lmfao",
            "pissin", "smut", "bitcher", "cumming", "fistfuckings", "lust", "pissing", "snatch", "bitchers", "cums", "fistfucks", "lusting", "pissoff", "son-of-a-bitch", "bitches",
            "cumshot", "flange", "m0f0", "poop", "spac", "bitchin", "cunilingus", "flog the log", "m0fo", "porn", "spunk", "bitching", "cunillingus", "fook", "m45terbate", "porno",
            "t1tt1e5", "bloody", "cunnilingus", "fooker", "ma5terb8", "pornography", "t1tties", "blow job", "cunt", "fuck hole", "ma5terbate", "pornos", "teets", "blow me",
            "cunt hair", "fuck puppet", "mafugly ", "prick", "teez", "blow mud", "cuntbag", "fuck trophy", "masochist", "pricks", "testical", "blowjob", "cuntlick", "fuck yo mama",
            "masterb8", "pron", "testicle", "blowjobs", "cuntlicker", "fuck", "masterbat*", "pube", "tit", "blue waffle", "cuntlicking", "fucka", "masterbat3", "pusse", "tit wank",
            "blumpkin", "cunts", "fuck-ass", "masterbate", "pussi", "titfuck", "boiolas", "cuntsicle", "fuck-bitch", "master-bate", "pussies", "tits", "bollock", "cunt-struck",
            "fucked", "masterbation", "pussy", "titt", "bollok", "cut rope", "fucker", "masterbations", "pussy fart", "tittie5", "boner", "cyalis", "fuckers", "masturbate",
            "pussy palace", "tittiefucker", "boob", "cyberfuc", "fuckhead", "mof0", "pussys", "titties", "boobs", "cyberfuck", "fuckheads", "mofo", "queaf", "tittyfuck", "booobs",
            "cyberfucked", "fuckin", "mo-fo", "queer", "tittywank", "boooobs", "cyberfucker", "fucking", "mothafuck", "rectum", "titwank", "booooobs", "cyberfuckers", "fuckings",
            "mothafucka", "retard", "tosser", "booooooobs", "cyberfucking", "fuckingshitmotherfucker", "mothafuckas", "rimjaw", "turd", "breasts", "d1ck", "fuckme", "mothafuckaz",
            "rimming", "tw4t", "buceta", "damn", "fuckmeat", "mothafucked", "bangsat", "twat", "bugger", "delaynomore", "fucks", "mothafucker", "bajingan", "twathead", "bum", "dick",
            "fucktoy", "mothafuckers", "goblok", "twatty", "bunny fucker", "dick hole", "fuckwhit", "mothafuckin", "gablek", "twunt", "bust a load", "dick shy", "fuckwit", "mothafucking",
            "sampah", "twunter", "busty", "dickhead", "fudge packer", "mothafuckings", "konek", "v14gra", "butt", "dildo", "fudgepacker", "mothafucks", "putangina", "v1gra", "butt fuck",
            "dildos", "fuk", "mother fucker", "tangina", "vagina", "butthole", "dink", "fuker", "mother fucker", "bobo", "viagra", "buttmuch", "dinks", "fukker", "motherfuck", "diu", "vulva",
            "buttplug", "dirsa", "fukkin", "motherfucked", "diule", "w00se", "CHIBAI", "dirty Sanchez", "fuks", "motherfucker", "diulelomo", "wang", "CCB", "dlck", "fukwhit", "motherfuckers",
            "diulelomohamgacan", "wank", "cheebye", "dog-fucker", "fukwit", "motherfuckin", "wanker", "cao cheebye", "doggie style", "fux", "motherfucking", "wanky", "cao chibai", "doggiestyle",
            "fux0r", "motherfuckings", "whoar", "nabe", "doggin", "gangbang", "motherfuckka", "suka", "whore", "kaninabe", "dogging", "gang-bang", "motherfucks", "suka blyat", "willies", "kaniniang",
            "donkeyribber", "gangbanged", "muff", "idi nahui", "willy", "knn", "doosh", "gangbangs", "muff puff", "wtf", "duche", "gassy ass", "mutha", "xrated", "dyke", "gaylord",
            "muthafecker", "perkele", "xxx", "gaysex", "muthafuckker", "vittu", "cao ni", "goatse", "muther", "saatana", "cao ni ma", "god damn", "mutherfucker", "ta ma de", "god-dam",
            "goddamn", "goddamned", "god-damned" };


        var vulgarArrU = vulgarArr.ToArray<String>();
        var inputName = usernameText.text;

        if(System.Array.IndexOf (vulgarArrU, inputName) != -1)
        {
                return false;
        }

        //we can do it the other way, is to check for inputName CONTAINS each and 
        //every element of the vulgarity array, but it will effect process time

        return true;
    }

    void SubmitUsername()
    {

        if (!isPassLocalVulgarityCheck())
        {
            DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_username_not_allowed"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
            return;
        }


        WWWForm form = new WWWForm();
        form.AddField("token", GameState.julesAccessToken);
        form.AddField("new_name", usernameText.text);
        DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_processing"), "", DialogueBoxController.Type.NoButtons);
        confirmButton.interactable = false;

		// HOWARD - Deprecated network code - TO REPLACE
//        // observe the http request
//        AppServer.CreatePost("me/username/update", form)
//        .Subscribe(
//            x => UpdateUsernameResult(x), // onSuccess
//            ex => AppServer.ErrorResponse(ex, "Error Update Username") // onError
//        );
    }

    void UpdateUsernameResult(string res)
    {
#if UNITY_EDITOR
        print("UpdateUsernameResult: " + res);
#endif

        confirmButton.interactable = true;

        JSONNode t = UpdateUsernamePassedCheck(res);
        if (t == null) // Null = has error.
        {
            return;
        }

        GameState.me.username = usernameText.text;

        SubmitPetCreation();
    }

    public JSONNode UpdateUsernamePassedCheck(string res)
    {
        JSONNode t = JSONNode.Parse(res);
        if (t != null && t["error"] != null)
        {
            string msg = t["msg"].Value;
            switch (msg)
            {
                case "Username already registered":
                    DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("error_username_already_in_use"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
                    return null;
            }
            DialogueBoxController.ShowMessage(msg, MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
            return null;
        }

        //No errors, return normal response.
        return t;
    }

    void SubmitPetCreation() {
#if UNITY_EDITOR
        Debug.Log("Save Pet Id: " + currentAvatarIndex);
        Debug.Log("Save bodyId Id: " + currentBodyIndex);
        Debug.Log("Save eyeId Id: " + currentEyeIndex);
#endif
        //HOWARD - Deprecated
        //if (GameState.julesAccessToken == null)
        //{
        //  //MainNavigationController.GoToStartMenu();
        //  MainNavigationController.GoToScene("CutScene_4");
        //  return;
        //}
        PlayerAvatar playerAvatar = new PlayerAvatar();

        Avatar a = new Avatar();
        playerAvatar.avatar = a;
        if (petAssetsList["pets"][currentAvatarIndex] != null)
        {

            a.id = currentAvatarIndex;
            a.prefab = petAssetsList["pets"][currentAvatarIndex]["prefab"];
            if (petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex] != null)
            {
                playerAvatar.avatarSkin.id = currentBodyIndex;
                playerAvatar.avatarSkin.prefab = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["prefab"];
            }
        }
        Buddy temp = GameState.playerBuddy;
        GameState.playerBuddy = new Buddy(usernameText.text,temp.expeditionCompleteTime, true, temp.time_required, temp.location, currentAvatarIndex, currentBodyIndex, currentEyeIndex,playerAvatar);
        GameState.me.Upload();
        AnalyticsSys.Instance.Report(REPORTING_TYPE.ChangedBuddy, petAssetsList["pets"][currentAvatarIndex]["name"] + " " + petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["name"]); 
        MainNavigationController.GoToHome();       
    }
    public void SubmitPetCreationCutscene()
    {
#if UNITY_EDITOR
        Debug.Log("Save Pet Id: " + currentAvatarIndex);
        Debug.Log("Save bodyId Id: " + currentBodyIndex);
        Debug.Log("Save eyeId Id: " + currentEyeIndex);
#endif
        cdc.TriggerContinue(); 
        PlayerAvatar playerAvatar = new PlayerAvatar();

        Avatar a = new Avatar();
        playerAvatar.avatar = a;
        if (petAssetsList["pets"][currentAvatarIndex] != null)
        {

            a.id = currentAvatarIndex;
            a.prefab = petAssetsList["pets"][currentAvatarIndex]["prefab"];
            if (petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex] != null)
            {
                playerAvatar.avatarSkin.id = currentBodyIndex;
                playerAvatar.avatarSkin.prefab = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["prefab"];
            }
        }
        GameState.playerBuddy = new Buddy(usernameText.text, 1, true, 0, 1, currentAvatarIndex, currentBodyIndex, currentEyeIndex, playerAvatar);
        GameState.me.Upload();
     //   MainNavigationController.GoToHome();
    }
    public void FinishName()
    {
        cdc.TriggerContinue();
    }
    public void FinishPetName()
    {
        if (!isPassLocalVulgarityCheck())
        {
            cdc.TriggerNo();
        }else
        {
            try {
                if (GameState.julesAccessToken == null)
                {
                    //MainNavigationController.GoToStartMenu();
                    MainNavigationController.GoToScene("CutScene_4");
                    return;
                }
                PlayerAvatar playerAvatar = new PlayerAvatar();

                Avatar a = new Avatar();
                playerAvatar.avatar = a;
                if (petAssetsList["pets"][currentAvatarIndex] != null)
                {

                    a.id = currentAvatarIndex;
                    a.prefab = petAssetsList["pets"][currentAvatarIndex]["prefab"];
                    if (petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex] != null)
                    {
                        playerAvatar.avatarSkin.id = currentBodyIndex;
                        playerAvatar.avatarSkin.prefab = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["prefab"];
                    }
                }
                GameState.playerBuddy = new Buddy(usernameText.text, 1, true, 0, 1, currentAvatarIndex, currentBodyIndex, currentEyeIndex, playerAvatar);
                GameState.me.Upload();
            }
            catch(System.Exception e)
            {

            }
            cdc.TriggerConfirm();
        }
    }

    void UpdateAvatarResult(string res)
    {
        print("UpdateAvatarResult: " + res);
        print("GameState.me.avatar.id: " + GameState.me.avatar.id);
        print("currentAvatarIndex: " + currentAvatarIndex);

        confirmButton.interactable = true;

        JSONNode t = JSONNode.Parse(res);
        if (t == null || t["Status"] == null || t["Status"] == "0") // Null = has error.
        {
            DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_creating_buddy"), MultiLanguage.getInstance().getString("dialog_box_error_title"));
            return;
        }

        //OK
        if (t["InsertId"] != null && t["InsertId"].AsInt != 0) {
            GameState.me.avatar = new PlayerAvatar();
            GameState.me.avatar.id = t["InsertId"].AsInt;
        }

        if (GameState.me.avatar != null)
        {
            GameState.me.avatar.avatarId = currentAvatarIndex;
            GameState.me.avatar.skinId = currentBodyIndex;
            GameState.me.avatar.eyeId = currentEyeIndex;
            if (petAssetsList["pets"][currentAvatarIndex] != null)
            {
                GameState.me.avatar.avatar.id = currentAvatarIndex;
                GameState.me.avatar.avatar.prefab = petAssetsList["pets"][currentAvatarIndex]["prefab"];
                if (petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex] != null)
                {
                    GameState.me.avatar.avatarSkin.id = currentBodyIndex;
                    GameState.me.avatar.avatarSkin.prefab = petAssetsList["pets"][currentAvatarIndex]["body"][currentBodyIndex]["prefab"];
                }
            }
        }

        WWWForm form = new WWWForm();
        form.AddField("user_id", GameState.me.id);
        form.AddField("username", GameState.me.username);
        form.AddField("buddy_name", usernameText.text);


        DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_processing"), "", DialogueBoxController.Type.NoButtons);
        confirmButton.interactable = false;

		// HOWARD - Deprecated network code - TO REPLACE
//        // observe the http request
//        if (GameState.playerBuddy == null)
//        {
//            AppServer.CreatePost("me/buddy/create", form)
//            .Subscribe(
//                x => FinishCreatingBuddy(x, usernameText.text), // onSuccess
//                ex => AppServer.ErrorResponse(ex, "Error Create Buddy") // onError
//            );
//        }else
//        {
//            AppServer.CreatePost("me/buddy/update", form)
//            .Subscribe(
//                x => FinishCreatingBuddy(x, usernameText.text), // onSuccess
//                ex => AppServer.ErrorResponse(ex, "Error Update Buddy") // onError
//            );
//        }
    }
    public void FinishCreatingBuddy(string x,string y)
    {

        MainNavigationController.GoToHome();
    }

    private void PlayPetSFX(string petType)
    {
        AudioSystem.PlaySFX("creation/" + petType);
    }

    public void Close()
    {
        errorPopup.SetActive(false);
    }

	/*

	public void RefreshDisplay()
	{
		petNameText.text = PetController.current.petAssetsList["pets"][currentPetIndex]["name"];
		bodyNameText.text = PetController.current.petAssetsList["pets"][currentPetIndex]["body"][currentBodyIndex]["name"];
		eyeNameText.text = (currentEyeIndex + 1).ToString();
		LoadPet();
	}

	void LoadPetEye()
	{
		PetController.current.LoadPetEye(currentEyeIndex);
		eyeNameText.text = (currentEyeIndex + 1).ToString();
	}

	public void SelectNextEye()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");

		if (currentEyeIndex >= PetController.current.petAssetsList["eyes"].Count - 1)
		{
			currentEyeIndex = 0;
		}
		else
		{
			currentEyeIndex++;
		}
		LoadPetEye();
	}

	public void SelectPrevEye()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");

		if (currentEyeIndex <= 0)
		{
			currentEyeIndex = PetController.current.petAssetsList["eyes"].Count - 1;
		}
		else
		{
			currentEyeIndex--;
		}
		LoadPetEye();
	}

	public void ResetPetClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");

		PlayerPrefs.DeleteKey("pet_id");
		PlayerPrefs.DeleteKey("pet_body_id");
		PlayerPrefs.DeleteKey("pet_eye_id");
		PlayerPrefs.DeleteKey("happiness_level");
		PlayerPrefs.DeleteKey("food_level");
		PlayerPrefs.DeleteKey("hygiene_level");

		GameState.me = new Player();
		currentPetIndex = 0;
		currentBodyIndex = 0;
		currentEyeIndex = 0;
		LoadController.current.ShowAccountCreation();
		LoadPet();
	}*/
}
