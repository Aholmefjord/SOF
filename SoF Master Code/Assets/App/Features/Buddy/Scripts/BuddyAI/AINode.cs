using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AINode : MonoBehaviour {
    public List<AINode> SurroundingNodes;
    public GameObject MovementTarget;
    bool NeedsSetup = true;
    public bool canBeVisited = true;
	// Use this for initialization
	void Start () {
        if (NeedsSetup)
        {
            MovementTarget = transform.GetChild(0).gameObject;
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.0f);// (transform.position, transform.localScale * 2, new Vector3(0, 0, 0));
            for (int i = 0; i < hits.Length; i++)
            {
                GameObject ColGO = hits[i].gameObject;
                if (ColGO.tag == "AINavigationNode" && ColGO != this.gameObject)
                {
                    SurroundingNodes.Add(ColGO.GetComponent<AINode>());
                }//else if(ColGO.tag == "Furniture" || ColGO.tag == "Editables")
              //  {
                //    canBeVisited = false;
                //    Debug.Log("Node cannot be visited");
              //  }
            }
        }
        
	}
	public bool AStarFindTarget(List<AINode> nodesVisited,AINode targetToFind,ref List<List<AINode>> pathways)
    {

        nodesVisited.Add(this);
        //Check to see if we are the target (usually only gets called at the start of the search)
        if(this == targetToFind)
        {
            pathways.Add(nodesVisited);            
            return true;
        }

        bool result = false;
        if (canBeVisited)
        {
            for (int i = 0; i < SurroundingNodes.Count; i++)
            {
                if (!checkVisited(SurroundingNodes[i], nodesVisited))
                {
                    if (SurroundingNodes[i] == targetToFind)
                    {
                        nodesVisited.Add(targetToFind);
                        pathways.Add(nodesVisited);
                        return true;
                    }
                    else //Didn't find target here, continue searching
                    {
                        // Debug.Log("Visiting Neighbour");
                        result = SurroundingNodes[i].AStarFindTarget(nodesVisited, targetToFind, ref pathways);
                        if (result)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return result;
    }
    public bool checkVisited(AINode searchObject,List<AINode> visitedNode)
    {
        for(int i = 0; i < visitedNode.Count; i++)
        {
            //Debug.Log("Found Out Visited");
            if (visitedNode[i] == searchObject) return true;
        }
        //Debug.Log("Found out it didn't visit");
        return false;
    }

	// Update is called once per frame
	void Update () {
        if (NeedsSetup)
        {
            NeedsSetup = false;
            MovementTarget = transform.GetChild(0).gameObject;
            
        }
    }
    public bool checkCanBeVisited()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2.0f);// (transform.position, transform.localScale * 2, new Vector3(0, 0, 0));
        bool localcanBeVisited = true;
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject ColGO = hits[i].gameObject;
            if (ColGO.tag == "AINavigationNode" && ColGO != this.gameObject)
            {

            }
            else
            {
                localcanBeVisited = false;
            }
        }
        canBeVisited = localcanBeVisited;
        return localcanBeVisited;
    }
}
