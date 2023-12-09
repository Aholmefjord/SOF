using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonAnimatorController : MonoBehaviour {
    public int buttonNumber;
    public Crab crabAssigned;
	// Use this for initialization
	void ResetForMe()
    {
        switch (buttonNumber)
        {
            case 1:
                transform.parent.GetComponent<CrabResetter>().button1 = true;
                break;
            case 2:
                transform.parent.GetComponent<CrabResetter>().button2 = true;
                break;
            case 3:
                transform.parent.GetComponent<CrabResetter>().button3 = true;
                break;
            case 4:
                transform.parent.GetComponent<CrabResetter>().button4 = true;
                break;
        }

        GetComponent<Button>().interactable = true;
    }
    public void ResetAnimator()
    {
        Debug.Log(gameObject.name);
        GetComponent<Animator>().SetTrigger("Reset");
        crabAssigned.GetComponent<Animator>().SetTrigger("reset");


        GetComponent<Button>().interactable = false;
    }
}
