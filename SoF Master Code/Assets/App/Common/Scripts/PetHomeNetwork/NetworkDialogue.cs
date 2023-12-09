using UnityEngine;
using System.Collections;

public class NetworkDialogue : MonoBehaviour {

	private GameObject senderChar;
	public void followRecordedPos(string senderName){
		this.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		//senderChar = new GameObject ();
		GameObject[] playersInGame = GameObject.FindGameObjectsWithTag("Avatar");
		foreach (GameObject player in playersInGame) {
			if(player.GetComponent<PhotonView>().owner.name == senderName){
				senderChar = player;
			}
		}
		StartCoroutine(startFollowPos());
		StartCoroutine(startDestroy());
	}
	private IEnumerator startFollowPos(){
		while (true) {
			if (senderChar) {
				this.transform.position = new Vector3 (Camera.main.WorldToScreenPoint (senderChar.transform.position).x,
					                                       Camera.main.WorldToScreenPoint (senderChar.transform.position).y + 100,
					                                       Camera.main.WorldToScreenPoint (senderChar.transform.position).z);
			}
			yield return new WaitForSeconds (0.01f);
		}
	}
	private IEnumerator startDestroy(){
		yield return new WaitForSeconds (2.0f);

		Destroy (gameObject);

	}
	
}
