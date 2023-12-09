using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TsumGame : MonoBehaviour {

    GameObject[] Icons;
    List<GameObject> SpecialIcons = new List<GameObject>();
    List<Transform> templist = new List<Transform>();

    public GameObject[] IconPrefabs;
    public LinkedList<GameObject> IconSequence = new LinkedList<GameObject>();
    List<int> arrayOfInts = new List<int>();
    bool temptest = false;
    bool temptimer = false;
    bool usehammer = false;
    public Transform SpawnPosition;
    public LineRenderer DragTrail;
    public GameObject GlowCircle;
    public GameObject HammerIndicator;
    public ProgressBar PowerupBar;
    public GameObject HammerGlow;
    public GameObject Arrow;
    public GameObject Vacuum;
    public GameObject SpecialTsumPos;
    public GameObject currentstagenumber;
    public GameObject TutorialBlock;
    public GameObject SettingBlock;
    public Transform TsumTsumRoot;
    public AnimatedNumber ScoreText;
    public AnimatedNumber AmountLeftText;
    float temptime = 0;
    public AnimatedNumber SpecialsRemainingText;
    public bool lastspecial = false;


    public Text ComboText;
    GameScoreChart gcs;
    public AudioClip[] fantasticClip;
    public AudioClip[] wonderfulClip;
    public AudioClip[] eggcellentClip;
    public AudioClip[] greatjobClip;
    AudioClip[][] encouragement;
    public string[] startAudio;
    public Text levelText;
    bool hammerUsed = false;
    bool shouldCountdown = false;
    float COUNTDOWN_TIMER = 3.0f;
    float LastMoveCountdownToCheckIfCannotBeSolved;
    bool tutorialguide = false;
    bool guidingarrow = false;
    const float LinkingRadius = 2.4f;
    const int NumberOfTsumsLeftInOrderToWin = 10;

    public float speed = 5;
    bool GameInProgress = true;
    public GameObject pausePanel;

    int NumberOfIcons;
    int totalIconToSpawn;
    int specialsleft;
    int counterToSpawnSpecial = 0;
    int counterToSpawnNextSpecial = 0;
    bool shouldRandomiseSpecial = true;
    int specialSpawnMin = 7;
    int specialSpawnMax = 10;

    bool shouldRandomise = false;

    [SerializeField]
    List<GameObject> levelBackgroundPrefabs = new List<GameObject>();
    [SerializeField]
    GameObject specialTsumPrefab;
    [SerializeField]
    GameObject tumbleBoomPrefab;
    [SerializeField]
    GameObject bigExplosionPrefab;
    [SerializeField]
    GameObject CFXM3_Hit_Light_B_Air;
    [SerializeField]
    GameObject CFXM3_Hit_Fire_A_Air;

    int NumberOfSpecials;
    GameObject stageClear, stageFailed,BlockHammerinput;
    private Dictionary<string, int> TsumScore = new Dictionary<string, int>();
    const int PowerPerTsum = 2; //KIT

    float prestartedTime;
    // Use this for initialization
    private void OnDestroy()
    {
        Time.fixedDeltaTime = prestartedTime;
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Time.fixedDeltaTime = prestartedTime;
        }
        else
        {
            prestartedTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0.025f;
        }
    }
    void Start()
    {
        tumbleTroubleComboChain.Load();
        tumbleTroubleDesign.Load();
        tumbleCritterRequirement.Load();
        tumbleCritterMultiplier.Load();

        InitializeObjects();
        LevelDesignData.LoadFileFromAssetBundle(Constants.TUMBLE_TROUBLE_CONFIG, "TsumTsumLevelDesign", ()=> {
            LevelDesignData.LoadLevel(GameState.tsumProg.selectedLevel);
        });
        NumberOfIcons = tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].IconCount;
        NumberOfSpecials = tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].SpecialIconCount;

        //int levelprefabno = GameState.tsumProg.selectedLevel % 6 + 1;
        int levelprefabno = 6;
        //GameObject.Instantiate(Resources.Load("Levels/Level" + levelprefabno));
        GameObject.Instantiate(levelBackgroundPrefabs[levelprefabno-1]);
        int maximumInitialSpawn = 55;
        int iconToSpawn;
        iconToSpawn = NumberOfIcons < maximumInitialSpawn ? NumberOfIcons : maximumInitialSpawn;

        totalIconToSpawn = NumberOfIcons;

        int levelContainer = GameState.tsumProg.selectedLevel + 1;
        if (levelContainer >= 10)
        {

            levelContainer /= 10;
            Mathf.Floor(levelContainer);
            specialSpawnMin += levelContainer;
            specialSpawnMin += levelContainer;

        }

        specialsleft = NumberOfSpecials;
        for (int i = 0; i < iconToSpawn; i++)
        {
            totalIconToSpawn--;
            counterToSpawnSpecial++;
            if (shouldRandomiseSpecial)
            {
                shouldRandomiseSpecial = false;
                counterToSpawnNextSpecial = Random.Range(specialSpawnMin, specialSpawnMax);
            }
            if (counterToSpawnSpecial == counterToSpawnNextSpecial && specialsleft > 0)
            {
                shouldRandomiseSpecial = true;
                counterToSpawnSpecial = 0;
                CreateTsumTsum(specialTsumPrefab, new Vector3(SpawnPosition.position.x, SpawnPosition.position.y, -1) + Cleanbox.RandomOffset(5.0f));//-1 to make it stand out from the rest of the critters
                specialsleft--;
                continue;
            }
            //HACK TO TEST       if (i % 1 == 0 && specialsleft > 0) { CreateTsumTsum("SpecialTsum", SpawnPosition.position + Cleanbox.RandomOffset(1.0f)); specialsleft--; }
            CreateRandomTsumTsum();
        }

        SetupUI();
        if (GameState.tsumProg.selectedLevel == 0)
        {
            tutorialguide = true;
            templist.Clear();
        }
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(GameObject.Find("Amount Left Container").FindChild("Left"), "tsum_tsum_game_left");
        MultiLanguage.getInstance().apply(GameObject.Find("Score Container").FindChild("Score Header"), "tsum_tsum_game_score_header");
        MultiLanguage.getInstance().apply(GameObject.Find("BtmRight Container").FindChild("Remaining"), "tsum_tsum_game_remaining");

        MultiLanguage.getInstance().apply(GameObject.Find("Combo Chain").FindChild("Combo"), "tsum_tsum_game_combo");

        MultiLanguage.getInstance().apply(stageClear.FindChild("Stage Clear Text").FindChild("Stage"), "tsum_tsum_game_stage_clear_stage_text");
        MultiLanguage.getInstance().apply(stageClear.FindChild("Stage Clear Text").FindChild("Clear"), "tsum_tsum_game_stage_clear_clear_text");
        MultiLanguage.getInstance().apply(stageFailed.FindChild("Stage Clear Text").FindChild("Stage"), "tsum_tsum_game_stage_failed_stage_text");
        MultiLanguage.getInstance().apply(stageFailed.FindChild("Stage Clear Text").FindChild("Failed"), "tsum_tsum_game_stage_failed_failed_text");

        MultiLanguage.getInstance().apply(pausePanel.FindChild("Text"), "in_game_menu_title");
        MultiLanguage.getInstance().apply(pausePanel.FindChild("CurrentStage"), "in_game_menu_current_stage");
        MultiLanguage.getInstance().apply(currentstagenumber.FindChild("Stage"), "tsum_stage_text");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Game Title").GetComponent<Image>(), "tsum_tsum_main_menu_title");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Resume").GetComponent<Image>(), "gui_settings_resume");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Replay").GetComponent<Image>(), "gui_settings_restart");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Exit").GetComponent<Image>(), "gui_settings_home");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Level").GetComponent<Image>(), "gui_settings_levelselect");

    }

    void InitializeObjects()
    {
        TutorialBlock.SetActive(false);
        prestartedTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime = 0.025f;

        currentstagenumber.FindChild("Text").SetText((GameState.tsumProg.selectedLevel + 1).ToString());
        LastMoveCountdownToCheckIfCannotBeSolved = COUNTDOWN_TIMER;
        gcs = gameObject.GetComponent<GameScoreChart>();
        AudioSystem.PlaySFX("buttons/" + startAudio[Random.Range(0, 2)].ToString());
        encouragement = new AudioClip[][] { eggcellentClip, greatjobClip, fantasticClip, wonderfulClip };
        GameInProgress = true;

        PowerupBar.SetValue(0, 0);
        GlowCircle.SetActive(false);
        ScoreText.SetValue(0);
        AmountLeftText.SetValue(NumberOfIcons - NumberOfTsumsLeftInOrderToWin);
        SpecialsRemainingText.SetValue(0);
        UpdateIconList = true;
        HammerGlow.SetActive(false);
        LastChainLength = 0;
        ChainedComboCount = 0;
        LongestChain = 0;

        stageClear = GameObject.Find("Stage Clear");
        stageClear.SetActive(false);
        stageFailed = GameObject.Find("Stage Failed");
        stageFailed.SetActive(false);
        BlockHammerinput = GameObject.Find("BlockInputHammer");
        BlockHammerinput.SetActive(false);
        HammerIndicator.SetActive(false);
        TsumScore.Add("Clammy", 100);
        TsumScore.Add("Puffy", 100);
        TsumScore.Add("Dumbo", 100);
        TsumScore.Add("Horsie", 100);
        TsumScore.Add("Hermie", 120);
        TsumScore.Add("Nautilass", 130);
        TsumScore.Add("Shelly", 140);
        TsumScore.Add("Prawnie", 150);
        TsumScore.Add("Krabby", 160);
        TsumScore.Add("Starry", 180);
        TsumScore.Add("SpecialTsum", 250);
    }

    void Countdown()
    {
        if (shouldCountdown)
        {
            if (LastMoveCountdownToCheckIfCannotBeSolved > 0)
                LastMoveCountdownToCheckIfCannotBeSolved -= Time.deltaTime;
            else
            {
                shouldCountdown = false;
                try
                {
                    if (GameInProgress)
                    {
                        if (!CheckIfMatchesArePossible())
                        {
                            GameFinish(0, false);
                        }
                    }
                }
                catch (System.Exception e)
                {

                }
            }
        }
    }

    public bool Drawing = true;
    public bool Broken = false;
    public bool UpdateIconList = false;

    bool startanimation = false;
    void Update()
    {

      
        if (Player.hasusedhammer==false && PowerupBar.GetValue() >= 1.0f)
        {
            BlockHammerinput.SetActive(true);
            HammerIndicator.SetActive(true);
            HammerGlow.SetActive(true);

          
           
            if(startanimation==false)
            {
                Player.hasusedhammer  = true;
                   
               
                //perma 
                DOTween.Sequence()
                .Append(HammerIndicator.transform.DOMove(HammerGlow.transform.position, 1f))
                .SetLoops(99999, LoopType.Yoyo)
                .AppendCallback(() =>
                {

                });
                startanimation = true;
            }
        
            //perma false;
            // set to false arrow+ block input after pressing
        }
        if(usehammer==true)
        {
            BlockHammerinput.SetActive(false);
            HammerIndicator.SetActive(false);
        }

        if (levelText.text != (GameState.tsumProg.selectedLevel + 1).ToString())
        {
            levelText.text = (GameState.tsumProg.selectedLevel + 1).ToString();
        }
        if (GameInProgress)
        {
            if(TutorialBlock.GetActive()==false&& SettingBlock.GetActive() == false)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MouseDown(clickpos);
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MouseMove(clickpos);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 clickpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    shouldRandomise = true;
                    arrayOfInts.Clear();
                    MouseUp(clickpos);
                }
            }
         
        }
        if (temptime <3)

            temptime += Time.deltaTime;

        if (temptime > 2.9f && temptimer == false)
        {
            Tutorialchaincheck();
            temptimer = true;

        }
        //   if (distSQ3 <2.2f * 2.2f)

        if (guidingarrow == true && templist.Count > 2 && temptest == false)
        {
            Arrow.SetActive(true); 

            DOTween.Sequence()
                     .SetLoops(99999, LoopType.Restart)
                 .AppendInterval(1.0f)
                 .AppendCallback(() =>
                 {
                     Arrow.GetComponent<SpriteRenderer>().DOColor(new Vector4(255f, 255f, 255f, 255f), 0.01f);
                     Arrow.transform.DOMove(templist[0].position, 0.01f);
                 })

                  .AppendInterval(0.5f)
                  .AppendCallback(() => Arrow.transform.DOMove(templist[1].position, 0.5f))
                    .AppendInterval(0.5f)
                  .AppendCallback(() =>
                  {
                    Arrow.GetComponent<SpriteRenderer>().DOColor(new Vector4(0f, 0f, 0f, 0f), 1f);
                      Arrow.transform.DOMove(templist[2].position, 1.0f);
                  }
        
                
                
                  
                  );
            temptest = true;
        }
        else if (guidingarrow == false)
        {
            Arrow.SetActive(false);
        }
        //possible rechk  method 
        // require to chk less per frame 
        //if(temptest=true&& templist.Count>2&&guidingarrow==true)
        //{
         
        //    Vector3 offset1 = (templist[0].transform.position - templist[1].transform.position); offset1.z = 0;
        //    float distsq1 = offset1.sqrMagnitude;
        //    Vector3 offset2 = (templist[1].transform.position - templist[2].transform.position); offset2.z = 0;
        //    float distsq2 = offset2.sqrMagnitude;
        //    if (distsq1 > 2.2f * 2.2f||distsq2>2.2f*2.2f)
        //        {
        //        Tutorialchaincheck();
        //        tutorialguide = true;
    
        //       }
        //    }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(1))
        {
            //var stars = PointsToStars (ScoreText.GetValue ());
            //GameFinish (ScoreText.GetValue (), stars);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log(CheckIfMatchesArePossible());
        }
#endif
      
    }


    //CHECK FOR LOSS, 3 SECONDS AFTER EVERY MOVE
    void FixedUpdate()
    {
        Countdown();
        //if (LastMoveCountdownToCheckIfCannotBeSolved > 0)
        //{
        //    LastMoveCountdownToCheckIfCannotBeSolved--;
        //    if (LastMoveCountdownToCheckIfCannotBeSolved == 0)
        //    {
        //        if (CheckIfMatchesArePossible() == false)
        //        {
        //            //LOSE THE GAME
        //            GameFinish(0, 0, false);
        //        }
        //    }
        //}

    }


    private int PointsToStars(int points)
    {
        //if (points > LevelDesignData.GetData("ScoreThreshold_3Star").AsInt) return 3;
        //if (points > LevelDesignData.GetData("ScoreThreshold_2Star").AsInt) return 2;
        //return 1;
        if (points >= tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].ScoreThreshold_3Star)
            return 3;
        if (points >= tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].ScoreThreshold_2Star)
            return 2;
        return 1;
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0)) {
            GameFinish(0, false);
        }
        //UPDATE ICON LIST
        if (UpdateIconList) {
            SpecialIcons.Clear();
            //UPDATE ICON LIST
            Icons = GameObject.FindGameObjectsWithTag("Icon");

            foreach (GameObject go in Icons) {
                if (go.name == "SpecialTsum") {
                    SpecialIcons.Add(go);
                }
            }
            UpdateIconList = false;

            //COUNT SPECIAL ICONS
            int specialcount = 0;
            foreach (GameObject go in Icons) {
                if (go.transform.position.y < -400f) {
                    GameObject.Destroy(go); UpdateIconList = true;
                }
                if (go.name == "SpecialTsum") specialcount++;
            }
            SpecialsRemainingText.SetValue(specialcount + specialsleft);
            if (SpecialsRemainingText.GetValue() > 0) {
                lastspecial = true;
            }

            int amountleft = (Icons.Length + totalIconToSpawn) - NumberOfTsumsLeftInOrderToWin;
            if (amountleft < 0) amountleft = 0;
            AmountLeftText.SetValue(amountleft);

            if (!CheckIfMatchesArePossible()) {
                if ((specialcount + specialsleft) == 0 &&
                    Icons.Length <= NumberOfTsumsLeftInOrderToWin) {
                    bool gameWon = true;
                    DOTween.Sequence()
                        .AppendInterval(1.0f)
                        .AppendCallback(() => GameFinish(ScoreText.GetValue(), gameWon));
                }
            }
        }
    }
    //****************************************
    //      MOUSEDOWN
    //****************************************
    void MouseDown(Vector3 clickpos)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //BEGIN DRAG IF ON ICON
        GameObject startingicon = FindNearest(clickpos);
        IconSequence.Clear();

        if (startingicon != null) {
            IconSequence.AddLast(startingicon);
            Drawing = true;
            Broken = false;
        }
    }
    //****************************************
    //      MOUSEMOVE
    //****************************************
    void MouseMove(Vector3 clickpos)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        GameObject currenticon = FindNearest(clickpos);
        if (currenticon != null) {
            GlowCircle.transform.position = currenticon.transform.position;
            GlowCircle.SetActive(true);
        } else {
            GlowCircle.SetActive(false);
        }

        bool updateline = false;
        if (currenticon != null &&                                                  //The current mouseover icon is valid
            IconSequence.Count > 0 && IconSequence.Last.Value != currenticon &&     //Not the hovering over the same icon at the end of queue
            IconSequence.Last.Value.GetComponent<SpriteRenderer>().sprite == currenticon.GetComponent<SpriteRenderer>().sprite && //Check for same icon type
            (IconSequence.Last.Value.transform.position - currenticon.transform.position).sqrMagnitude < LinkingRadius * LinkingRadius //Within Range
            )
        {
            //Check if backtracking
            if (IconSequence.Count >= 2 && IconSequence.Last.Previous.Value == currenticon)
            {
                IconSequence.RemoveLast();
                updateline = true;
            }
            else if (IconSequence.Contains(currenticon) == false && SpecialIcons.Contains(currenticon) == false)
            {
                IconSequence.AddLast(currenticon);
                updateline = true;
            }
            foreach (GameObject go in SpecialIcons)
            {
                go.GetComponent<CounterToDestroyPearl>().shouldHighlight = false;
            }
            GameObject[] affectedPearls = AnyPearlsNearLastIcon(IconSequence.Last.Value.transform.position, LinkingRadius).ToArray();
            if (IconSequence.Count >= 3)
            {
                for (int i = 0; i < affectedPearls.Length; i++)
                {
                    affectedPearls[i].GetComponent<CounterToDestroyPearl>().shouldHighlight = true;
                }
            }
            Debug.LogError("affectedpearls count: " + affectedPearls.Length);
            if (updateline)
            {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_CLICK_02");
                DragTrail.SetVertexCount(IconSequence.Count);
                int i = 0;
                foreach (GameObject icon in IconSequence)
                {
                    DragTrail.SetPosition(i, new Vector3(icon.transform.position.x, icon.transform.position.y, -1));
                    i++;
                }
            }
        }// if currenticon 
    }

    //****************************************
    //      MOUSEUP
    //****************************************
    string[] EncouragementWords = { "AWESOME!", "GREAT JOB!", "FANTASTIC!", "WONDERFUL!" };
    int LastChainLength = 0;
    int ChainedComboCount = 0;
    int LongestChain = 0;

    /// <summary>
    /// on mouse release, calculate score
    /// </summary>
    /// <param name="clickpos"></param>
    void MouseUp(Vector3 clickpos)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        List<GameObject> DistinctIconSequence = IconSequence.Distinct().ToList();
        foreach (GameObject go in SpecialIcons) {
            go.GetComponent<CounterToDestroyPearl>().shouldHighlight = false;
        }

        //CHECK FINAL TSUM
        GameObject currenticon = FindNearest(clickpos);
        if (currenticon == null ||                                                  //The current mouseover icon is valid
            IconSequence.Count <= 0 || IconSequence.Last.Value == null ||
            IconSequence.Last.Value.GetComponent<SpriteRenderer>().sprite != currenticon.GetComponent<SpriteRenderer>().sprite || //Check for same icon type
            (IconSequence.Last.Value.transform.position - currenticon.transform.position).sqrMagnitude > LinkingRadius * LinkingRadius //Within Range
            )
        {
            Broken = true;
        }

        int randomWord;

        //UPDATE COMBOS
        if (DistinctIconSequence.Count >= 3 && Broken == false)
        {
            LastMoveCountdownToCheckIfCannotBeSolved = COUNTDOWN_TIMER;
            shouldCountdown = true;
            int iconNo;
            try {
                iconNo = DistinctIconSequence[0].GetComponent<IconIdentifier>().iconNo;
            } catch (System.Exception e) {
                iconNo = -1;
            }

            //Update Critters collected list
            UpdateCrittersCollected(DistinctIconSequence[0].name, DistinctIconSequence.Count, iconNo);

            //Count glowing, previously 5x tsums
            int glowcount = 0;
            //AnyPearlNearSoonToBeDestroyedIconRadius(IconSequence.Last.Value.transform.position, LinkingRadius);
            GameObject[] affectedPearlss = AnyPearlsNearLastIcon(IconSequence.Last.Value.transform.position, LinkingRadius).ToArray();
            foreach (GameObject go in affectedPearlss) {
                go.GetComponent<CounterToDestroyPearl>().shouldCounterincrease = true;
            }
            foreach (GameObject icon in IconSequence) {
                if (icon.transform.childCount > 0) glowcount++;
            }
            foreach (GameObject go in SpecialIcons) {
                if (go.GetComponent<CounterToDestroyPearl>().shouldCounterincrease) {
                    go.GetComponent<CounterToDestroyPearl>().shouldCounterincrease = false;
                    go.GetComponent<CounterToDestroyPearl>().counter++;
                    Camera.main.GetComponent<CameraShake>().StartCoroutine("ShakeCamera");
                }
                if (go.GetComponent<CounterToDestroyPearl>().counter == go.GetComponent<CounterToDestroyPearl>().counterToDestroy) {
                    //go.GetComponent<CounterToDestroyPearl>().SpawnOnDestroy();
                    DestroyIcon(go, bigExplosionPrefab);
                }
            }

            //Update Chained Combos
            if (LastChainLength > 3 && IconSequence.Count > 3) {
                ChainedComboCount++;
            } else {
                ChainedComboCount = 0;
            }
            if (IconSequence.Count > LongestChain) LongestChain = IconSequence.Count;
            LastChainLength = IconSequence.Count;

            string combotext = "";
            float chainmultiplier = CalculateChainComboMultiplier();
            if (chainmultiplier == 1.0f) {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_BURST_A");
            }
            if (chainmultiplier == 1.1f) {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_BURST_B_Chain1X1");
            }
            if (chainmultiplier == 1.2f) {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_BURST_C_Chain1X2");
            }
            if (chainmultiplier == 1.3f) {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_BURST_D_Chain1X3");
            }
            if (chainmultiplier >= 1.4f) {
                AudioSystem.PlaySFX("buttons/JULES_TUMBLE_BURST_E_Chain1X4");
            }
            Debug.LogError("chainmultiplier: " + chainmultiplier);
            if (chainmultiplier > 1.0f) {
                combotext = MultiLanguage.getInstance().getString("tsum_tsum_game_combo_chain") + chainmultiplier.ToString("#.#");
            } else if (IconSequence.Count >= 4) combotext = MultiLanguage.getInstance().getString("tsum_tsum_game_combo_multi") + IconSequence.Count;

            if (combotext != "") {
                ComboText.text = combotext;
                ComboText.DOKill(true);
                ComboText.color = new Color(1f, 1f, 1f, 1f);
                ComboText.transform.DOPunchScale(Vector3.one * 1.05f, 0.5f, 2);
                ComboText.DOFade(0f, 2.0f).SetDelay(0.8f);
            }

            if (IconSequence.Count >= 5) { 
                //Debug.LogError("IconSequence.Count >= 5");
                AudioSystem.PlaySFX("buttons/JULES_CC_ULTIMATE_BURST_02");
                randomWord = Random.Range(1, 5);// EncouragementWords.Length); 
                TextRiser.Create("ScoreText", MultiLanguage.getInstance().getString("tsum_tsum_game_encouragementwords_words_" + randomWord), Camera.main.WorldToScreenPoint(IconSequence.First.Value.transform.position));
                randomWord -= 1;
                // becuz string to get encouragement word is 1-2-3-4 while sound clip is 0-1-2-3
                var audioclipref = encouragement[randomWord][Random.Range(0, encouragement[randomWord].Length)];//this is the bug
                if (audioclipref != null) {
                    var audioclipName = audioclipref.name;
                    AudioSystem.PlaySFX("TumbleTrouble/encouragement/" + encouragement[randomWord][Random.Range(0, encouragement[randomWord].Length)].name);
                } else {
                    Debug.Log("hang");
                }
            }

            //CLEAR EIGHT---------------------------
            if (IconSequence.Count >= 8) {
                //Debug.LogError("IconSequence.Count >= 8");
                Detonate(IconSequence.Last.Value.transform.position, LinkingRadius, bigExplosionPrefab);
                foreach (GameObject icon in IconSequence) {
                    DestroyIcon(icon, bigExplosionPrefab);
                    //CreateRandomTsumTsum();
                }
            } else
            //CLEAR FIVE---------------------------
            //else if (IconSequence.Count == 5)
            //{
            //    int count = 0;
            //    foreach (GameObject icon in IconSequence)
            //    {
            //        if (count != 2)
            //        {
            //            DestroyIcon(icon, "BiggerExplosion");
            //            //CreateRandomTsumTsum();
            //        }
            //        else
            //        {
            //            GameObject glow = (GameObject)GameObject.Instantiate(Resources.Load("Glow"), icon.transform.position, Quaternion.identity);
            //            glow.transform.parent = icon.transform;
            //        }
            //        count++;
            //    }

            //}
            //ORDINARY CLEAR---------------------------

            {
                foreach (GameObject icon in IconSequence) {
                    DestroyIcon(icon, CFXM3_Hit_Light_B_Air);
                    guidingarrow = false;
                }
            }
            UpdateIconList = true;

            var iconProgress = GameState.tsumProg.GetCritter(currenticon.name);
            float[] critterMultiplierList;
            float critterMultiplier = 1f;
            //tumbleCritterMultiplier.critterMultiplier[iconProgress.level - 1];
            if (iconNo != -1) {
                critterMultiplierList = tumbleCritterMultiplier.critterMultiplier[iconNo - 1].ScoreMultiplier.ToArray();
                critterMultiplier = critterMultiplierList[iconProgress.level - 1];
            }

            try {
                int scoreearned = (int)(TsumScore[currenticon.name] * critterMultiplier); //* DistinctIconSequence.Count;
                float tsumlengthmultiplier = 1.0f + (DistinctIconSequence.Count - 3) * 0.4f;
                if (tsumlengthmultiplier < 1.0f) tsumlengthmultiplier = 1.0f;
                if (tsumlengthmultiplier > 2.0f) tsumlengthmultiplier = 2.0f;
                tsumlengthmultiplier *= CalculateChainComboMultiplier();

                scoreearned = (int)((float)scoreearned * tsumlengthmultiplier);
                scoreearned *= (int)Mathf.Pow(2, glowcount); //ADD BONUS FROM SPECIALS
                ScoreText.Add(scoreearned);
                TextRiser.Create("ScoreText", scoreearned.ToString(), Input.mousePosition);

                float previousvalue = PowerupBar.GetValue();
                DOTween.Sequence()
                    .AppendInterval(1f)
                    .AppendCallback(() => {
                        PowerupBar.SetValue(PowerupBar.GetValue() + DistinctIconSequence.Count * (PowerPerTsum / 100f), 1.0f);
                        if (previousvalue < 1.0f && PowerupBar.GetValue() >= 1.0f) {
                            PowerupBar.transform.GetChild(0).GetComponent<Image>().DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                            HammerGlow.SetActive(true);
                        }
                    });
            } catch(KeyNotFoundException e) {
                Debug.LogException(e);
                Debug.LogError("which key: "+currenticon.name);
                System.Text.StringBuilder build = new System.Text.StringBuilder();
                foreach(var tet in TsumScore) {
                    build.Append(tet.Key);
                    build.Append(", ");
                    build.AppendLine(tet.Value.ToString());
                }
                Debug.LogError(build);
            }
        }

        IconSequence.Clear();
        Drawing = false;
        Broken = false;
        DragTrail.SetVertexCount(0);
        GlowCircle.SetActive(false);
    }
    GameObject GetIconPrefab(string name)
    {
        for(int i =0; i < IconPrefabs.Length; ++i) {
            if (IconPrefabs[i].name.Equals(name)) {
                return IconPrefabs[i];
            }
        }
        return null;
    }
    void DestroyIcon(GameObject icon, GameObject effectPrefab)
    {
        GameObject effect = (GameObject)GameObject.Instantiate(effectPrefab, icon.transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
        //this is the vacuum part 
        //instatiate same object remove collider and tag then animate and destory 
        if (icon.name != "SpecialTsum") {
            GameObject visual = (GameObject)GameObject.Instantiate(GetIconPrefab(icon.name), icon.transform.position, Quaternion.identity);

            visual.GetComponent<CircleCollider2D>().enabled = false;
            visual.transform.tag = "Untagged";

            //remove the chkes in icons 
            DOTween.Sequence()
                  .AppendCallback(() => visual.transform.DOMove(Vacuum.transform.position, 1.0f))
              .AppendInterval(0.5f)
                  .AppendCallback(() => {
                      visual.transform.DOScale(0f, 0.5f).SetEase(Ease.Linear);
                  });

            Destroy(visual, 1.0f);
            Destroy(effect, 1.5f);
        }

        if (icon.name == "SpecialTsum") {
            if (SpecialsRemainingText.GetValue() == 1 && lastspecial == true) {
                GameObject oyster = (GameObject)GameObject.Instantiate(specialTsumPrefab, icon.transform.position, Quaternion.identity);
                
                oyster.GetComponent<CircleCollider2D>().enabled = false;
                oyster.transform.tag = "Untagged";
                DOTween.Sequence()
                        .AppendCallback(() => oyster.transform.DOMove(SpecialTsumPos.transform.position, 1.0f))
                        .AppendCallback(() => {

                        });
                Destroy(oyster, 1.0f);

                DOTween.Sequence()
                      .AppendCallback(() => oyster.transform.DOMove(SpecialTsumPos.transform.position, 1.0f))
                      .AppendInterval(1.0f)
                      .AppendCallback(() => {
                          GameObject speicaleffect = Instantiate(tumbleBoomPrefab, SpecialTsumPos.transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
                          Destroy(speicaleffect, 1.0f);
                      });
            }
        }

        Destroy(icon);
        UpdateIconList = true;
        if (totalIconToSpawn > 0) {
            totalIconToSpawn--;
            counterToSpawnSpecial++;

            if (shouldRandomiseSpecial) {
                shouldRandomiseSpecial = false;
                counterToSpawnNextSpecial = Random.Range(specialSpawnMin, specialSpawnMax);
            }

            if (counterToSpawnSpecial == counterToSpawnNextSpecial && specialsleft > 0) {
                shouldRandomiseSpecial = true;
                counterToSpawnSpecial = 0;
                CreateTsumTsum(specialTsumPrefab, new Vector3(SpawnPosition.position.x, SpawnPosition.position.y, -1) + Cleanbox.RandomOffset(5.0f));//-1 to make it stand out from the rest of the critters
                specialsleft--;
                return;
            }
            CreateRandomTsumTsum(true);
        }
    }

    List<GameObject> AnyPearlsNearLastIcon(Vector3 center, float radius)
    {
        List<GameObject> affectedPearls = new List<GameObject>();
        GameObject bestTarget = null;
        //float closestDistanceSqr = radius * radius;
        float closestDistanceSqr = Mathf.Pow(radius, 2);
        Vector3 currentPosition = center;
        foreach (GameObject potentialTarget in SpecialIcons)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                bestTarget = potentialTarget;
                affectedPearls.Add(bestTarget);
            }
        }
        return affectedPearls;
    }

    void Detonate(Vector3 position, float radius, GameObject effectPrefab)
    {
        int destroyedcount = 0;
        int specialcount = 0;
        int scoreearned = 0;
        foreach (GameObject go in Icons)
        {
            Vector3 offset = (position - go.transform.position);
            offset.z = 0;
            float distSQ = offset.sqrMagnitude;
            if (distSQ < radius * radius)
            {
                if (go.name == "SpecialTsum")
                    continue;

                destroyedcount++;
                DestroyIcon(go, effectPrefab);
                //CreateRandomTsumTsum();
                if (go.transform.childCount > 0) specialcount++;
                scoreearned += TsumScore[go.name];
                //Debug.LogError("score: " + scoreearned);
            }
        }

        scoreearned *= (int)Mathf.Pow(2, specialcount); //ADD BONUS FROM SPECIALS
        ScoreText.Add(scoreearned);
        if (destroyedcount > AmountLeftText.CurrentValue) destroyedcount = AmountLeftText.CurrentValue;
        AmountLeftText.Add(-destroyedcount);

        float previousvalue = PowerupBar.GetValue();
        DOTween.Sequence()
         .AppendInterval(1f)
        
         .AppendCallback(() =>
         {
             PowerupBar.SetValue(PowerupBar.GetValue() + destroyedcount * (PowerPerTsum / 100f), 1.0f);
             if (previousvalue < 1.0f && PowerupBar.GetValue() >= 1.0f)
             {
                 PowerupBar.transform.GetChild(0).GetComponent<Image>().DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                 HammerGlow.SetActive(true);
             }
         });

        if (hammerUsed)
        {
            TextRiser.Create("ScoreText", (scoreearned).ToString(), Camera.main.WorldToScreenPoint(position));
            hammerUsed = false;
        }
    }

    void GameFinish(int points_earned, bool gameWon)
    {
        float chainBonusScore = 0;
        if (GameInProgress == false) return;
        GameInProgress = false;
        stageClear.SetActive(gameWon);
        stageFailed.SetActive(!gameWon);

        if (gameWon) {
            int chainComboArrayPosition = LongestChain - 3;
            if (chainComboArrayPosition < 0)
                chainComboArrayPosition = 0;
            else if (chainComboArrayPosition > 6)
                chainBonusScore = 11000 * (1 + ((float)LongestChain - 10) / 10);
            if ((LongestChain - 3) > 0 && chainComboArrayPosition <= 6)
            {
                chainBonusScore += Random.Range(tumbleTroubleComboChain.comboChain[chainComboArrayPosition].ComboChainMin, tumbleTroubleComboChain.comboChain[chainComboArrayPosition].ComboChainMax + 1);
            }
            AudioSystem.PlaySFX("buttons/JULES_WIN_SOUND_01");
            //points_earned += LevelDesignData.GetData("StageClearBonus_Score").AsInt;
            var currentLevelId = GameState.tsumProg.selectedLevel;
            var currentLevel = GameState.tsumProg.GetLevel(currentLevelId);

            var tsumTrials = PlayerPrefs.GetInt("tsum.playCount.level_" + GameState.tsumProg.selectedLevel, 0);
            ++tsumTrials;
            PlayerPrefs.SetInt("tsum.playCount.level_" + GameState.tsumProg.selectedLevel, tsumTrials);

            int gold = 0, rGem = 0, uGem = 0;
            if (GameState.tsumProg.GetLevel(currentLevelId).status == TsumPlayerProgression.ELevelStatus.Available)
            {
                points_earned += tumbleTroubleDesign.levelSettings[currentLevelId].StageClearBonus_Score;
                gold += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonusGold_Min, tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonusGold_Max + 1);
                rGem += tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
                uGem += tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
                //to send feedback when player first complete this level
                GameState.PostProgress(ActivityFeedIds.FEED_GAME_PROGRESS, currentLevelId + 1, GameNamesFeed.GAME_NAME_TUMBLE);
            }

            gold += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].GoldReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].GoldReward_Max + 1));
            rGem += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Max + 1));
            uGem += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Max + 1));

            var stars_earned = PointsToStars(Mathf.Max(currentLevel.pointsEarned, points_earned + (int)chainBonusScore));
            currentLevel.pointsEarned = Mathf.Max(currentLevel.pointsEarned, points_earned + (int)chainBonusScore);
            currentLevel.starEarned = stars_earned;

            if (GameState.tsumProg.GetLevel(currentLevelId).status == TsumPlayerProgression.ELevelStatus.Available && stars_earned == 3)
            {
                gold += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Gold;
                rGem += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
                uGem += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
            }
            bool loadBuddy = false;
            if ((currentLevelId + 1) % 5 == 0)
            {
                
                loadBuddy = true;
            }
            
            currentLevel.status = TsumPlayerProgression.ELevelStatus.Finished;
            currentLevel.needAnimation = true;
            GameState.tsumProg.SetLevel(currentLevelId, currentLevel);
            //if (tsumTrials == 1)
            //         {
            //             gold += Random.Range(LevelDesignData.GetData("FirstClearBonusGold_Min").AsInt, LevelDesignData.GetData("FirstClearBonusGold_Max").AsInt);
            //             rGem += LevelDesignData.GetData("FirstClearBonus_Jewel").AsInt;
            //         }
            //         if (tsumTrials < 20)
            //         {
            //             gold += Random.Range(LevelDesignData.GetData("GoldReward_Min").AsInt, LevelDesignData.GetData("GoldReward_Max").AsInt);
            //             rGem += Random.Range(LevelDesignData.GetData("JewelReward_Min").AsInt, LevelDesignData.GetData("JewelReward_Max").AsInt);
            //         }
            //         else
            //         {
            //             gold += Random.Range(LevelDesignData.GetData("DiminishedGoldReward_Min").AsInt, LevelDesignData.GetData("DiminishedGoldReward_Max").AsInt);
            //         }

            DOTween.Sequence()
            .AppendInterval(3f)
            .Join(stageClear.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(stageClear.transform.DOScale(0f, 1f).From().SetEase(Ease.OutBounce))

            .AppendCallback(() =>
            {
                WinScreenUI.Open(true, points_earned, (int)chainBonusScore, gold, rGem, uGem, loadBuddy);

                var nextLevel = GameState.tsumProg.GetLevel(currentLevelId + 1);
                if (nextLevel.status == TsumPlayerProgression.ELevelStatus.Locked)
                {
                    nextLevel.status = TsumPlayerProgression.ELevelStatus.Available;
                    nextLevel.needAnimation = true;
                    GameState.tsumProg.SetLevel(currentLevelId + 1, nextLevel);
                }
            });


        }
        else
        {
            AudioSystem.PlaySFX("buttons/" + failSFX[Random.Range(0, failSFX.Length)]);
            DOTween.Sequence()
            .AppendInterval(3f)
            .Join(stageFailed.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(stageFailed.transform.DOScale(0f, 1f).From().SetEase(Ease.OutBounce))

            .AppendCallback(() =>
            {
                WinScreenUI.Open(false);
            });
        }

        Destroy(gameObject, 4f);

        //HIEU BIND SCOREBOARD HERE
        //SceneManager.LoadScene("LevelSelect");

    }
    public string[] failSFX;

    void UpdateCrittersCollected(string name, int number, int iconNo = -1)
    {
        if (iconNo == -1)
        {
            return;
        }
        int[] temp = tumbleCritterRequirement.critterSettings[iconNo - 1].levelRequirement.ToArray();

        var critterData = GameState.tsumProg.GetCritter(name);

        critterData.points += number;
        critterData.maxPoints = temp[critterData.level - 1];

        if (critterData.points > critterData.maxPoints)
        {
            critterData.points -= critterData.maxPoints;
            critterData.level++;
        }
        GameState.tsumProg.SetCritter(name, critterData);
    }

    private float CalculateChainComboMultiplier()
    {
        float multiplier = 1.0f;
        if (ChainedComboCount > 10) return 2.0f;
        else if (ChainedComboCount >= 1)
        {
            return 1.0f + 0.1f * (ChainedComboCount);
            //return ((1.0f + 0.1f) * (ChainedComboCount - 1));
        }
        //else if (ChainedComboCount > 1) return 1.0f + 0.1f * (ChainedComboCount - 1);
        return multiplier;
    }


    private bool CheckIfMatchesArePossible()
    {
        //CAN USE HAMMER
        if (PowerupBar.GetValue() >= 1.0f) return true;

        //CHECK CHAIN OF 3
        foreach (GameObject go1 in Icons)
        {
            string tsum1name = go1.GetComponent<SpriteRenderer>().sprite.name;
            foreach (GameObject go2 in Icons)
            {
                //Check for different tsum
                if (go2 == go1) continue;
                if (go2.name == "SpecialTsum")
                    continue;
                //Check for different tsum type
                string tsum2name = go2.GetComponent<SpriteRenderer>().sprite.name;
                if (tsum2name != tsum1name) continue;

                //Check for too far away
                Vector3 offset = (go1.transform.position - go2.transform.position); offset.z = 0;
                float distSQ = offset.sqrMagnitude;
                if (distSQ > LinkingRadius * LinkingRadius) continue;

                //IT MATCHES, CHECK FOR 3RD TSUM:
                foreach (GameObject go3 in Icons)
                {
                    //Check for different tsum
                    if (go3 == go2 || go3 == go1) continue;
                    //Check for different tsum type
                    string tsum3name = go3.GetComponent<SpriteRenderer>().sprite.name;
                    if (tsum3name != tsum1name) continue;

                    //Check for too far away
                    Vector3 offset3 = (go3.transform.position - go2.transform.position); offset3.z = 0;
                    float distSQ3 = offset3.sqrMagnitude;
                    if (distSQ3 < LinkingRadius * LinkingRadius)
                    {
                        if (distSQ3 < LinkingRadius * LinkingRadius)


                            return true;
                    }

                }
            }
        }
        return false;
    }
    private bool Tutorialchaincheck()
    {
        //CAN USE HAMMER
        if (PowerupBar.GetValue() >= 1.0f) return true;

        //CHECK CHAIN OF 3
        foreach (GameObject go1 in Icons)
        {
            string tsum1name = go1.GetComponent<SpriteRenderer>().sprite.name;
            foreach (GameObject go2 in Icons)
            {
                //Check for different tsum
                if (go2 == go1) continue;
                if (go2.name == "SpecialTsum")
                    continue;
                //Check for different tsum type
                string tsum2name = go2.GetComponent<SpriteRenderer>().sprite.name;
                if (tsum2name != tsum1name) continue;

                //Check for too far away
                Vector3 offset = (go1.transform.position - go2.transform.position); offset.z = 0;
                float distSQ = offset.sqrMagnitude;
                if (distSQ > LinkingRadius * LinkingRadius) continue;

                //IT MATCHES, CHECK FOR 3RD TSUM:
                foreach (GameObject go3 in Icons)
                {
                    //Check for different tsum
                    if (go3 == go2 || go3 == go1) continue;
                    //Check for different tsum type
                    string tsum3name = go3.GetComponent<SpriteRenderer>().sprite.name;
                    if (tsum3name != tsum1name) continue;

                    //Check for too far away
                    Vector3 offset3 = (go3.transform.position - go2.transform.position); offset3.z = 0;
                    float distSQ3 = offset3.sqrMagnitude;
                    if (distSQ3 < LinkingRadius * LinkingRadius)
                    {
                        if (distSQ3 <2.1f * 2.1f)
                        {
                            if (tutorialguide == true)
                            {
                                DOTween.Sequence().AppendInterval(1f)
                                    .AppendCallback(() =>
                                    {
                                        // List<Vector3> templist = new List<Vector3>();
                                        Debug.Log("addding");
                                        templist.Add(go1.transform);
                                        templist.Add(go2.transform);
                                        templist.Add(go3.transform);
                                        tutorialguide = false;
                                        guidingarrow = true;
                                    });

                            }
                            return true;
                        }
                          
                    }

                }
            }
        }
        return false;
    }

    public void UseHammer()
    {
        usehammer = true;
        shouldRandomise = true;
        //Check if enough power
        if (PowerupBar.GetValue() < 1.0f) {
            //Popup text: Not enough power!
            TextRiser.Create("ScoreText", "NOT ENOUGH POWER", Input.mousePosition + Vector3.right * Screen.width * 0.36f);
            return;
        }
        LastMoveCountdownToCheckIfCannotBeSolved = COUNTDOWN_TIMER;
        shouldCountdown = true;
        hammerUsed = true;
        Camera.main.transform.DOShakePosition(0.5f, 0.2f, 30);
        
        GameObject IsolatedIcon = null;
        int min_isolatedneighbors = 10;
        foreach (GameObject go in Icons)
        {
            int matchingneighbors = 0;
            //        Sprite mysprite = go.GetComponent<SpriteRenderer>().sprite;
            //        if (mysprite.name == "SpecialTsum")
            //continue;

            if (go.name == "SpecialTsum")
                continue;
            foreach (GameObject neighbor in Icons) {
                if (go == neighbor) continue;

                Vector3 offset = (neighbor.transform.position - go.transform.position);
                offset.z = 0;
                float distSQ = offset.sqrMagnitude;
                if (distSQ < LinkingRadius * LinkingRadius) {
                    //if (mysprite == neighbor.GetComponent<SpriteRenderer>().sprite)
                    //{
                    //    matchingneighbors++;
                    //}

                    if (go.name == neighbor.name) {
                        matchingneighbors++;
                    }

                    //ignore things which have special tsums as a neighbor
                    if (go.name == "SpecialTsum") {
                        matchingneighbors = 1000;
                        break;
                    }
                }
            }

            if (matchingneighbors < min_isolatedneighbors) {
                IsolatedIcon = go;
                min_isolatedneighbors = matchingneighbors;
            }
        }

        if (IsolatedIcon != null)
        {
            Detonate(IsolatedIcon.transform.position, LinkingRadius, CFXM3_Hit_Fire_A_Air);
        }

        PowerupBar.transform.GetChild(0).GetComponent<Image>().DOKill(true);
        PowerupBar.transform.GetChild(0).GetComponent<Image>().color = new Color(73f / 255f, 246f / 255f, 218f / 255f, 1f);
        HammerGlow.SetActive(false);

        PowerupBar.SetValue(0, 1.0f);
        UpdateIconList = true;

    }

    GameObject FindNearest(Vector3 position)
    {
        //float nearestSQ = 3.0f * 3.0f;
        float nearestSQ = LinkingRadius * LinkingRadius;
        GameObject nearest = null;
        foreach (GameObject go in Icons) {
            Vector3 offset = (position - go.transform.position);
            offset.z = 0;
            float distSQ = offset.sqrMagnitude;
            if (distSQ < nearestSQ) {
                nearestSQ = distSQ;
                nearest = go;
            }
        }
        return nearest;
    }

    GameObject CreateRandomTsumTsum(bool gameStarted = false)
    {
        int iconToSpawnFrom;
        //string name = IconPrefabs[Random.Range(0, 3)].name;
        int[] iconTypes = tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].IconTypes.ToArray();
        if (gameStarted == false)
            iconToSpawnFrom = iconTypes[Random.Range(0, iconTypes.Length)];
        else {
            if (shouldRandomise) {
                int iconSpawnType1;
                int iconSpawnType2;
                int iconSpawnType3;
                shouldRandomise = false;
                iconSpawnType1 = iconTypes[Random.Range(0, iconTypes.Length)];
                iconSpawnType2 = iconTypes[Random.Range(0, iconTypes.Length)];
                iconSpawnType3 = iconTypes[Random.Range(0, iconTypes.Length)];
                while (iconSpawnType2 == iconSpawnType1) {
                    iconSpawnType2 = iconTypes[Random.Range(0, iconTypes.Length)];
                }
                while (iconSpawnType3 == iconSpawnType2 || iconSpawnType3 == iconSpawnType1) {
                    iconSpawnType3 = iconTypes[Random.Range(0, iconTypes.Length)];
                }
                if (tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].IconCount > 3) {
                    arrayOfInts.Add(iconSpawnType1);
                    arrayOfInts.Add(iconSpawnType2);
                    arrayOfInts.Add(iconSpawnType3);
                } else {
                    arrayOfInts.Add(iconSpawnType1);
                    arrayOfInts.Add(iconSpawnType2);
                }
            }

            //int iconToSpawnContainer = Random.Range(0, 3);
            //iconToSpawnFrom = iconToSpawnContainer == 0 ? iconSpawnType1 : iconSpawnType2;
            iconToSpawnFrom = arrayOfInts[Random.Range(0, arrayOfInts.Count)];
        }


        string name = IconPrefabs[iconToSpawnFrom - 1].name;

        Vector3 position = SpawnPosition.position + Cleanbox.RandomOffset(3.0f);
        return CreateTsumTsum(IconPrefabs[iconToSpawnFrom - 1], position);
    }

    /*
    GameObject CreateTsumTsum(string name, Vector3 position)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(name), position, Quaternion.identity, TsumTsumRoot);
        go.name = name;
        //go.transform.parent = TsumTsumRoot;
        return go;
    }//*/

    GameObject CreateTsumTsum(GameObject prefab, Vector3 position)
    {
        GameObject go = Instantiate(prefab, position, Quaternion.identity, TsumTsumRoot);
        go.name = prefab.name;
        return go;
    }

    public void ExitGameClick()
    {
        MainNavigationController.GotoMainMenu();
        if (Time.timeScale != 1.0f)
            Time.timeScale = 1.0f;
    }
    public void LevelSelectClick()
    {
        //MainNavigationController.GoToTumbleTroubleLevelSelect();
        GoToTsumTsumLevelSelect();
        if (Time.timeScale != 1.0f)
            Time.timeScale = 1.0f;
    }

    public void PauseAnimationClick()
    {
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);
    }

    public void ResumeAnimationClick()
    {
        Time.timeScale = 1.0f;
        pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
    

    public void ReloadGameClick()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1.0f;

        //SceneManager.UnloadScene("Tsum");
        //SceneManager.LoadScene("Tsum");
        GotoTsumTsumGame();
    }

    public static void GoToTsumTsumLevelSelect()
    {
        MainNavigationController.DoAssetBundleLoadLevel(Constants.TUMBLE_TROUBLE_SCENES, "TsumTsum Main Screen");
        MainMenuUI.opentsumui = true;
    }
    public static void GotoTsumTsumGame()
    {
        MainNavigationController.DoAssetBundleLoadLevel(Constants.TUMBLE_TROUBLE_SCENES, "Tsum");
    }
}

