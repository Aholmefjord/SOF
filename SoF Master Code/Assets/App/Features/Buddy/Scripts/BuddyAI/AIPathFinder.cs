using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class AIPathFinder : MonoBehaviour {
    public List<AINode> aiNodes;
    public GameObject gridNode;
    // Use this for initialization

    List<GameObject> waypoints = new List<GameObject>();

    bool _threadRunning;
    Thread _thread;
    public bool tryFindNewNode = false;
    WalkingAvatar avatar;
    void Start () {
        avatar = GetComponent<WalkingAvatar>();
        aiNodes = new List<AINode>();
        gridNode = GameObject.Find("Grid");
        GameObject[] nodeGOs = GameObject.FindGameObjectsWithTag("AINavigationNode");
        for(int i = 0; i < nodeGOs.Length; i++)
        {
            aiNodes.Add(nodeGOs[i].GetComponent<AINode>());
            waypoints.Add(nodeGOs[i]);
        }
        Random.seed = (int)Time.timeSinceLevelLoad;
	}
    public AINode transStart;
    public AINode transTarget;
    public List<AINode>[] pathsToTarget;
    List<List<AINode>> pathwaysUsed;
    public List<AINode> shortestPath;
	// Update is called once per frame
	void Update () {
        if (tryFindNewNode)
        {
            
        }
	}
    bool hasFinishedTask = false;

    void OnFinished()
    {
        //Debug.Log("Found a path");
        avatar.currentPathway = shortestPath;
    }
    public void BeginFindingNewPath(AINode startNode)
    {
        //Debug.Log("found a target");
        tryFindNewNode = false;
        if(startNode == null)
        transStart = aiNodes[Random.Range(0, aiNodes.Count)];
        bool found = false;

        while (!found)
        {
            transTarget = aiNodes[Random.Range(0, aiNodes.Count)];
            found = transTarget.canBeVisited;
        }
        _thread = new Thread(ThreadedWork);
        _thread.Start();
    }
    void FindNewPath()
    {

        pathwaysUsed = new List<List<AINode>>();
        pathsToTarget = pathwaysUsed.ToArray();
        if (transStart.AStarFindTarget(new List<AINode>(), transTarget, ref pathwaysUsed))
        {
            int indexMinimal = pathwaysUsed[0].Count;
            int indexMin = 0;
            for (int i = 0; i < pathwaysUsed.Count; i++)
            {
                if (pathwaysUsed[i].Count < indexMinimal)
                {
                    indexMinimal = pathwaysUsed[i].Count;
                    indexMin = i;
                }
            }
            shortestPath = pathwaysUsed[indexMin];
    //        Debug.Log("Found Target");
        }
    }

    public GameObject GetRandomWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Count)];
    }

    void ThreadedWork()
    {
        _threadRunning = true;
        bool workDone = false;

        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
            //Debug.Log("Still Searching");
            // Do Work...
            FindNewPath();
            OnFinished();
            workDone = true;
        }
        _threadRunning = false;
    }

    void OnDisabled()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }
}
