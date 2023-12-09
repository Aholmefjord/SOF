using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class EmojiManager : MonoBehaviour {

	private Dictionary <string,string> emojiPresentation;
	private Dictionary <string,Texture> emojiImagePool;
	public GameObject panelPrefab;

	public void init(GameObject panelPrefab){
		emojiPresentation = new Dictionary<string,string>();
		emojiImagePool = new Dictionary<string,Texture>();
		this.panelPrefab = panelPrefab;
	}
	public void loadJSONStringToEmojiData(){
		TextAsset emoData = (TextAsset)Resources.Load("data/emoji", typeof(TextAsset));
		Debug.Log(emoData.text);
		JSONArray emoAssetsList = JSONNode.Parse(emoData.text) as JSONArray;
		foreach (JSONNode emoNode in emoAssetsList)
		{
			addEmoji(emoNode["Name"].Value,emoNode["Symbol"].Value);
		}
		Debug.Log (emojiPresentation);
	}

	public void setEmojiChatPanels(){

	//	GameObject emojiPanelContainer = GameObject.Find ("EmojiList/PanelContainer");
	//	foreach(KeyValuePair<string, string> emojiIndividualPanelData in emojiPresentation)
	//	{
	//		setEmojiPool(emojiIndividualPanelData.Key);
	//		GameObject instantiatedPanel = Instantiate(panelPrefab) as GameObject;
	//		instantiatedPanel.transform.SetParent(emojiPanelContainer.transform,false);
	//		instantiatedPanel.GetComponent<EmojiPanel>().setNameAndSymbol(emojiIndividualPanelData.Key,emojiIndividualPanelData.Value);
//		}
	}
	private void setEmojiPool(string name){
		Texture tempEmojiTexture = Resources.Load ("emoticons/" + name) as Texture;
		emojiImagePool.Add (name, tempEmojiTexture);
	}
	public Texture getEmojiTextureFromPool(string name){
		return emojiImagePool [name];
	}
	public void addEmoji(string name, string symbol){
		emojiPresentation.Add (name, symbol);
	}
	public Dictionary <string,string> getEmojiDict(){
		return emojiPresentation;
	}
}
