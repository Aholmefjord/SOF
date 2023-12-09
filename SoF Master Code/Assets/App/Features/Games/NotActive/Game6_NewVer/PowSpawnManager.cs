using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowSpawnManager : MonoBehaviour {
    public PowGameController pgc;
    public List<GameObject> GOToSpawn;
    public float cooldownTimerMax;
    float currentCooldown;
    int shieldLeft;
    public bool isEnemy;

    private List<float> Ypositions;
    // Use this for initialization
    void Start ()
    {
        shieldLeft = 3;
        
        Ypositions = new List<float>();

        Ypositions.Add(-3.25f);
        Ypositions.Add(-2.5f);
        Ypositions.Add(-1.75f);
        Ypositions.Add(-1);
    }

	public virtual void TakeDamage(int amount)
    {
        if (shieldLeft > 0)
        {
            shieldLeft--;
        }
        else if (isEnemy)
        {
            PowGameController.EnemyHealth -= amount;
        }
        else
        {
            PowGameController.Player.RemoveHealth(amount);
        }
           
    }
    public virtual void SpawnOnClick(GameObject goSpawn)
    {

        GameObject go = GameObject.Instantiate<GameObject>(goSpawn);
        GameObject G = GameObject.Find("ScrollableStage");
        go.transform.parent = G.transform.parent;      
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        int randomValue = (int)Random.Range(0, Ypositions.Count);
        go.transform.position = transform.position + new Vector3(0, Ypositions[randomValue], randomValue);
        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        go.GetComponent<PowBaseCharacter>().moveDirection = (isEnemy) ? 1 : -1;

        PowGameController.friendlyFishPool.Add(go);
    }
	// Update is called once per frame
	public virtual void Update () {
        if (!PowGameController.isPaused)
        {
            if (GOToSpawn.Count > 0) {
                currentCooldown += Time.deltaTime;
                if (currentCooldown > cooldownTimerMax)
                {
                    currentCooldown = 0;
                    GameObject go = Instantiate<GameObject>(GOToSpawn[0]);
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    go.transform.position = transform.position;
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    go.GetComponent<PowBaseCharacter>().moveDirection = (isEnemy)? 1 : -1;                    
                    GOToSpawn.RemoveAt(0);
                    
                    PowGameController.friendlyFishPool.Add(go);
                }
            }
        }
	}
}
