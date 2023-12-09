using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BuddyFaceExpressions : MonoBehaviour {
    public GameObject face_object;
    public Texture NeutralEye;
    public Texture NeutralMouth;

    public Texture happyEyeTexture;
    public Texture sadEyeTexture;
    public Texture cryEyeTexture;
    public Texture sianEyeTexture;
    public Texture angryEyeTexture;
    public Texture sillyEyeTexture;
    public Texture squintEyeTexture;
    public Texture closeEyeTexture;

    public Texture openMouthTexture;
    public Texture closeMouthTexture;
    public Texture chewOneTexture;
    public Texture chewTwoTexture;
    public Texture sianMouthTexture;
    public Texture smirkMouthTexture;
    public Texture squigglyMouthTexture;
    public Texture whistleMouthTexture;
    public Texture disgustMouthTexture;
    public Texture frownMouthTexture;
    public Texture laughMouthTexture;
    public Texture spiralEyeTexture;

    private Animator mAnim;

    private AnimationNode currentAnim;
    private AnimationNode prevAnim;
    private int frameCounter = 0;

    private Hashtable animationNodeList;

    // Use this for initialization
    void Start() {

        init();

        mAnim = GetComponent<Animator>();
    }

    private void init()
    {
        //Load xml file containing information of what we have for language files and their related file locations
        String fileContent = Cleanbox.LoadTextFile("Animation/facial_expression");
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(fileContent);

        //make sure we successfully load the XML file
        if (xml.FirstChild != null)
        {
            //new file location list
            animationNodeList = new Hashtable();

            //the root content is the last child. <Document>
            XmlNode rootNode = xml.LastChild;

            //Go through the list
            for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
            {
                //Each node here contains an animation node
                XmlNode itemNode = rootNode.ChildNodes[i];

                //Item content is what we wanted for our langauge name and file locations
                AnimationNode newAnimationNode = new AnimationNode();

                newAnimationNode.name = itemNode.Attributes["name"].Value;
                newAnimationNode.animateNodeList = new List<AnimateNode>();

                //Debug.Log("Creating AnimationNode: " + newAnimationNode.name);

                for (int k = 0; k < itemNode.ChildNodes.Count; ++k)
                {
                    XmlNode subItemNode = itemNode.ChildNodes[k];
                    
                    //Now we need to get the animate node out
                    AnimateNode newAnimateNode = new AnimateNode();
                    
                    newAnimateNode.frame = int.Parse(subItemNode.Attributes["frame"].Value);
                    newAnimateNode.expression = subItemNode.Attributes["expression"].Value;

                    string parts = subItemNode.Attributes["parts"].Value;

                    if (parts.Equals("eyes"))
                    {
                        newAnimateNode.part = AnimateNode.PARTS.EYES;
                    }
                    else if (parts.Equals("mouth"))
                    {
                        newAnimateNode.part = AnimateNode.PARTS.MOUTH;
                    }

                    //Check if this is a loop
                    if (subItemNode.Attributes["loop"] != null)
                    {
                        newAnimateNode.isLoop = bool.Parse(subItemNode.Attributes["loop"].Value);

                        if (newAnimateNode.isLoop)
                            newAnimateNode.loopFrame = int.Parse(subItemNode.Attributes["loop_frame"].Value);
                    }
                    else
                    {
                        newAnimateNode.isLoop = false;
                    }

                    newAnimationNode.animateNodeList.Add(newAnimateNode);
                }


                //Debug.Log("Finish AnimationNode: " + newAnimationNode.name);

                //Now add the new style information into our language and it's file locations
                //Using language name as the key.
                animationNodeList.Add(itemNode.Attributes["name"].Value, newAnimationNode);
            }
        }
        else
        {
            Debug.LogError("Error loading buddy face expression xml");
        }
    }

    private void Update()
    {
        if (prevAnim.name == null || currentAnim.name == null)
            return;
                
        //easier to find out about if a new animation starts
        if (!prevAnim.name.Equals(currentAnim.name))
        {
            frameCounter = 0;
            prevAnim = currentAnim;
        }

        updateCurrentAnimation(frameCounter);
        frameCounter++;
    }

    private void updateCurrentAnimation(int frameCounter)
    {
        //What we want to do here is as following
        //Check against every AnimateNode in current animation
        //Compare the frame to the frame counter
        //Once the frame and frame counter match
        //We will perform the action requried, which is switching expression on the buddy
        for (int i = 0; i < currentAnim.animateNodeList.Count; ++i)
        {
            //Get child node
            AnimateNode node = (AnimateNode)currentAnim.animateNodeList[i];

            //Compare frame
            if (node.frame == frameCounter && !node.isLoop)
            {
                applyAnimate(node);
            }

            //if it's loop we will do things differently
            if (node.isLoop)
            {
                //frame is now the start frame of this loop
                if (frameCounter >= node.frame)
                {
                    //start of the loop
                    //should always appy
                    if(frameCounter == node.frame)
                        applyAnimate(node);

                    //and then every alternative frame
                    if((frameCounter-node.frame)%node.loopFrame==0)
                        applyAnimate(node);
                }
            }
        }
    }

    private void applyAnimate(AnimateNode node)
    {
        //need to get the texutre according to the expression name
        if (node.part == AnimateNode.PARTS.EYES)
        {
            if (node.expression.Equals("neutral"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = NeutralEye;
            else if (node.expression.Equals("happy"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = happyEyeTexture;
            else if (node.expression.Equals("squint"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = squintEyeTexture;
            else if (node.expression.Equals("close"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = closeEyeTexture;
            else if (node.expression.Equals("angry"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = angryEyeTexture;
            else if (node.expression.Equals("spiral"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = spiralEyeTexture;
        }
        else if (node.part == AnimateNode.PARTS.MOUTH)
        {
            if (node.expression.Equals("neutral"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = NeutralMouth;
            else if (node.expression.Equals("open"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = openMouthTexture;
            else if (node.expression.Equals("chew1"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = chewOneTexture;
            else if (node.expression.Equals("chew2"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = chewTwoTexture;
            else if (node.expression.Equals("laugh"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = laughMouthTexture;
            else if (node.expression.Equals("smirk"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = smirkMouthTexture;
            else if (node.expression.Equals("sian"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = sianMouthTexture;
            else if (node.expression.Equals("disgust"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = disgustMouthTexture;
            else if (node.expression.Equals("squiggly"))
                face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = squigglyMouthTexture;
        }
    }
    
    public void TriggerNeutral()
    {
        face_object.GetComponent<SkinnedMeshRenderer>().materials[2].mainTexture = NeutralEye;
        face_object.GetComponent<SkinnedMeshRenderer>().materials[3].mainTexture = NeutralMouth;
    }

    //Change animation state based on name
    //Cons, please make sure you spell the name correctly.
    public void StartAnimation(string name)
    {
        currentAnim = GetAnimationNode(name);

        if (prevAnim.name == null)
            prevAnim = currentAnim;
        
        if(currentAnim.name == null)
        {
            Debug.LogError("Start Animation, name is not been registered: " + name);
        }
    }

    private AnimationNode GetAnimationNode(string name)
    {
        return (AnimationNode)animationNodeList[name];
    }

    struct AnimationNode
    {
        public string name;
        public List<AnimateNode> animateNodeList;
    }

    struct AnimateNode
    {
        public enum PARTS
        {
            EYES = 0,
            MOUTH
        }
        
        public int frame;
        public PARTS part;
        public string expression;

        public bool isLoop;
        public int loopFrame;   //loop on every how many frames
    }
}
