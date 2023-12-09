using UnityEngine;
using System.Collections;

public class IQTestButton : MonoBehaviour {
    public IQTest iqTest;
    public bool isCorrect;
	// Use this for initialization
	
	// Update is called once per frame
	public void OnClick () {
        iqTest.AnswerQuestion(isCorrect);
	}
    public void InstantiateTest()
    {
        int indexToInstantiate = 1;
        //need some logic here to determine which test needs to be conducted, for now we check status of first test;
        if(GameState.me.iqTestTwoResults == "{null}")
        {
            indexToInstantiate = 2;
        }else if(GameState.me.iqTestThreeResults == "{null}")
        {
            indexToInstantiate = 3;
        }
        GameObject b = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ui/IQTest"));
        b.GetComponent<IQTest>().Start(indexToInstantiate);
    }
}
