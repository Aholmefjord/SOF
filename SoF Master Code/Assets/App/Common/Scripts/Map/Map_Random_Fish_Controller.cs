using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map_Random_Fish_Controller : MonoBehaviour {

    [SerializeField]
    float SharkTimer = 6.0f;

    [SerializeField]
    float WhaleTimer = 8.0f;

    [SerializeField]
    float LargeFishTimer = 5.5f;

    [SerializeField]
    float SchoolTimer = 5.0f;

    [SerializeField]
    Vector3 UpperLeftRange;

    [SerializeField]
    Vector3 SchoolLowerRightRange;

    [SerializeField]
    Vector3 LowerRightRange;

    [SerializeField]
    GameObject TargetCanvas;

    [SerializeField]
    GameObject SharkPrefab;

    [SerializeField]
    GameObject WhalePrefab;

    [SerializeField]
    GameObject LargeFishPrefab;

    [SerializeField]
    GameObject SchoolPrefab;

    private float p_SavedSharkTimer;
    private float p_SavedWhaleTimer;
    private float p_LargeFishTimer;
    private float p_SchoolTimer;

    //List<GameObject> FishContainer;

	// Use this for initialization
	void Start () {
        // Save the specified timer
        p_SavedSharkTimer = SharkTimer;
        p_SavedWhaleTimer = WhaleTimer;
        p_LargeFishTimer = LargeFishTimer;
        p_SchoolTimer = SchoolTimer;

        //FishContainer = new List<GameObject>();
}
	
	// Update is called once per frame
	void Update () {
        float storeDelta = Time.deltaTime;

        // Update delta time
        SharkTimer -= storeDelta;
        WhaleTimer -= storeDelta;
        LargeFishTimer -= storeDelta;
        SchoolTimer -= storeDelta;

        SpawnFish(SharkTimer, SharkPrefab, false);
        SpawnFish(WhaleTimer, WhalePrefab, false);
        SpawnFish(LargeFishTimer, LargeFishPrefab, false);
        SpawnFish(SchoolTimer, SchoolPrefab, true);

        RefreshTimers();

        CleanUp();

    }

    private void SpawnFish(float timer, GameObject prefabobj, bool background)
    {
        if (timer <= 0.0f)
        {
            int Left = ((int)Random.Range(0, 1.9f));
            Vector3 bkgrnd;

            if (background)
                bkgrnd = SchoolLowerRightRange;
            else
                bkgrnd = LowerRightRange;

            GameObject tmp;

            // Spawn on the Right
            if (Left > 0)
            {
                tmp = (GameObject)Instantiate(prefabobj, new Vector3(LowerRightRange.x, Random.Range(UpperLeftRange.y, bkgrnd.y), 0.0f), Quaternion.Euler(0, 0, 0));
                tmp.transform.SetParent(TargetCanvas.transform);
                tmp.GetComponent<Map_Fish_Movement>().SetDirection(false);
            }

            // Spawn on the Left
            else
            {
                tmp = (GameObject)Instantiate(prefabobj, new Vector3(UpperLeftRange.x, Random.Range(UpperLeftRange.y, bkgrnd.y), 0.0f), Quaternion.Euler(0, 0, 0));
                tmp.transform.SetParent(TargetCanvas.transform);
                tmp.GetComponent<Map_Fish_Movement>().SetDirection(true);
                RectTransform tmpRect = tmp.GetComponent<RectTransform>();
                tmp.GetComponent<RectTransform>().localScale = new Vector3(-tmpRect.localScale.x, tmpRect.localScale.y, tmpRect.localScale.z);
            }

            //FishContainer.Add(tmp);
        }
    }

    private void RefreshTimers()
    {
        if (SharkTimer <= 0.0f)
            SharkTimer = p_SavedSharkTimer;
        if (WhaleTimer <= 0.0f)
            WhaleTimer = p_SavedWhaleTimer;
        if (LargeFishTimer <= 0.0f)
            LargeFishTimer = p_LargeFishTimer;
        if (SchoolTimer <= 0.0f)
            SchoolTimer = p_SchoolTimer;
    }

    private void CleanUp()
    {
        GameObject[] tmpContainer = GameObject.FindGameObjectsWithTag("PropFish");

        foreach(GameObject x in tmpContainer)
        {
            if(x.transform.position.x > LowerRightRange.x)
            {
                //FishContainer.Remove(x);
                Destroy(x);
                continue;
            }
            
            else if(x.transform.position.x < UpperLeftRange.x)
            {
                //FishContainer.Remove(x);
                Destroy(x);
                continue;
            }
        }
    }
}
