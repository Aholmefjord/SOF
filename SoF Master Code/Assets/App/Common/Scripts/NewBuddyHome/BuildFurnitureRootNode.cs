using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BuildFurnitureRootNode : MonoBehaviour {

    public List<BuildFurnitureNode> nodeList;
    public int nodeAt;
    void Start()
    {
        if (nodeList.Count <= 0)
        {
            nodeList = new List<BuildFurnitureNode>(gameObject.GetComponentsInChildren<BuildFurnitureNode>());
        }
    }
    public BuildFurnitureNode selectNewNode(int nodeAt)
    {
        this.nodeAt = nodeAt;
        return nodeList[nodeAt];
    }
    public void SelectFurniture()
    {

    }
    public int GetCurrentlySelectedNode()
    {
        return nodeAt;
    }
}
