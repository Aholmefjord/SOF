using UnityEngine;
using System;
using System.Collections.Generic;


public class UIManager {
	Dictionary<string, GameObject> templates = new Dictionary<string, GameObject> ();
	Dictionary<string, GameObject> children = new Dictionary<string, GameObject> ();
	GameObject uiRoot_;
	public GameObject UIRoot {
		get {
			return uiRoot_;
		}
	}
	public UIManager (GameObject uiRoot) {
		Init (uiRoot);
	}
	public void Init (GameObject uiRoot) {
		uiRoot_ = uiRoot;
		ProcessTree (uiRoot.transform, uiRoot.transform);
		foreach (KeyValuePair<string, GameObject> pair in templates) {
			pair.Value.SetActive (false);
			pair.Value.transform.SetParent (uiRoot.transform, false);
			pair.Value.transform.localPosition = Vector3.zero;
		}
	}
	public GameObject this[string id] {
		get {
			GameObject found = null;
			children.TryGetValue (id, out found);
			return found;
		}
	}
	public GameObject GetTemplate (string id) {
		return templates[id];
	}
	void ProcessTree (Transform trans, Transform parent, bool isProcessingTemplate = false) {
		string key = trans.name;
		if (trans.tag == "UIIgnore") {
			GameObject.Destroy (trans.gameObject);
			return;
		} else if (trans.tag == "UITemplate") {
			if (templates.ContainsKey (key)) {
				Debug.LogWarning ("Duplicate for UITemplate: " + key);
			}
			templates[key] = trans.gameObject;
			isProcessingTemplate = true;
		}
		if (!isProcessingTemplate) {
			children[key] = trans.gameObject;
		}
		foreach (Transform child in trans) {
			ProcessTree (child, parent, isProcessingTemplate);
		}
	}
	public bool HasTemplate (string templateName) {
		return templates.ContainsKey (templateName);
	}
	public GameObject CreateFromTemplate (string templateName, GameObject parent = null) {
		if (parent == null) {
			parent = UIRoot;
		}
		GameObject template = templates[templateName];
		if (parent == null)
			parent = uiRoot_;

        GameObject created = GameObject.Instantiate (template);
		created.transform.SetParent (parent.transform, false);
		created.SetActive (true);
		return created;
	}
}