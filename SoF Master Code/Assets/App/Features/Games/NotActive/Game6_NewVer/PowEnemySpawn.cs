using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowEnemySpawn: MonoBehaviour
{
    public List<List<GameObject>> objectsToSpawn;
    public List<List<int>> objectSpawnRate;
    float objectTimer = 0;
    // Use this for initialization
    int spawnWave = 0;
    public int WaveAt = 1;
    public float timeBetweenSpawns = 0;
    public float packSpawnTimer = 0;
    public int packNumber = 1;
    public int initialSpawnCount = 1;

    private List<float> Ypositions;
	void Start () {
        Ypositions = new List<float>();

        Ypositions.Add(-3.25f);
        Ypositions.Add(-2.5f);
        Ypositions.Add(-1.75f);
        Ypositions.Add(-1);
    }
// these two are read only references for the multi wave instances;
    public List<List<GameObject>> objectSpawnInternal;
    public List<List<int>> objectSpawnRateInternal;
    bool updated = false;
    public void Update()
    {
        if (!updated)
        {
            objectSpawnInternal = new List<List<GameObject>>();
            objectSpawnRateInternal = new List<List<int>>();
            for(int i = 0; i < objectsToSpawn.Count; i++)
            {
                GameObject[] b = objectsToSpawn[i].ToArray();
                objectSpawnInternal.Add(new List<GameObject>(b));
            }
            for (int i = 0; i < objectSpawnRate.Count; i++)
            {
                int[] b = objectSpawnRate[i].ToArray();
                objectSpawnRateInternal.Add(new List<int>(b));
            }
            updated = true;
        }
        try {
            if(!(PowGameController.isPaused || PowGameController.isFinished)) {
                if(objectSpawnRate.Count > 0)
                {
                    if(objectSpawnRate[0].Count > 0)
                    {
                        if (!GameObject.Find("TakoGame").GetComponent<PowGameController>().dialogueSequenceOver)
                        {
                            if(PowGameController.enemyFishPool.Count < (initialSpawnCount + 1))
                                objectTimer += Time.deltaTime * 10;
                            else
                                return;
                            //switch (levelNumber)
                            //{
                            //    case 1:
                            //        // spawn 1st enemy
                            //        if (PowGameController.enemyFishPool.Count < 2)
                            //            objectTimer += Time.deltaTime;
                            //        else
                            //            return;
                            //        break;
                            //    case 2:
                            //        // spawn 1st enemy
                            //        if (PowGameController.enemyFishPool.Count < 2)
                            //            objectTimer += Time.deltaTime;
                            //        else
                            //            return;
                            //        break;
                            //    case 3:
                            //        // spawn 
                            //        if (PowGameController.enemyFishPool.Count < 3)
                            //            objectTimer += Time.deltaTime;
                            //        else
                            //            return;
                            //        break;
                            //}
                        }
                        else
                            objectTimer += Time.deltaTime;
                        if(objectTimer > objectSpawnRate[spawnWave][0]) {
                            objectTimer = 0;
                            GameObject go = GameObject.Instantiate<GameObject>(objectsToSpawn[0][0]);
                            GameObject G = GameObject.Find("ScrollableStage");
                            go.transform.parent = G.transform.parent;
                            PowBaseCharacterStats pbcs = new PowBaseCharacterStats(go.GetComponent<PowBaseCharacter>().FileToLoadStatsFrom);
                            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                            int randomValue = (int)Random.Range(0, Ypositions.Count);
                            go.transform.position = transform.position + new Vector3(0, Ypositions[randomValue], randomValue);
                            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                            objectsToSpawn[0].RemoveAt(0);
                            objectSpawnRate[0].RemoveAt(0);

                            PowGameController.enemyFishPool.Add(go);
                        }
                    }else
                    {
                        if (objectSpawnRate.Count > 0)
                        {
                                objectSpawnRate.RemoveAt(0);
                                objectsToSpawn.RemoveAt(0);
                        }else
                        {
                        }
                    }
                }else if (packNumber > 0)
                {
                    timeBetweenSpawns += Time.deltaTime;
                    if (timeBetweenSpawns > packSpawnTimer)
                    {

                        WaveAt += 1;
                        timeBetweenSpawns = 0;
                        packNumber--;
                        objectsToSpawn = new List<List<GameObject>>();
                        objectSpawnRate = new List<List<int>>();
                        for (int i = 0; i < objectSpawnInternal.Count; i++)
                        {
                            GameObject[] b = objectSpawnInternal[i].ToArray();
                            objectsToSpawn.Add(new List<GameObject>(b));
                        }
                        for (int i = 0; i < objectSpawnRateInternal.Count; i++)
                        {
                            int[] b = objectSpawnRateInternal[i].ToArray();
                            objectSpawnRate.Add(new List<int>(b));
                        }
                    }
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
    }
    // Update is called once per frame

}

