using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to schedule callbacks that required to be run on the main Unity thread (to use Unity API)
/// It will schedule the callback to run in the next available Update (or FixedUpdate) call
/// </summary>
public class AsyncCallbackQueue : MonoBehaviour {

	private static AsyncCallbackQueue _instance;
	public static AsyncCallbackQueue Instance { get { return _instance != null ? _instance : CreateInstance (); } }
	public static AsyncCallbackQueue CreateInstance () {
		if (_instance ==null) {
			GameObject go = new GameObject ();
			_instance = go.AddComponent<AsyncCallbackQueue> ();
			go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}
		return _instance;
	}

	private void Awake () {
		if (_instance == null || _instance == this) {
			_instance = this;
		} else {
			Destroy (this);
		}
	}

	private void OnDestroy () {
		if (_instance == this) {
			_instance = null;
		}
	}

	private Queue<Action> updateCallbacks = new Queue<Action> ();
	private Queue<Action> fixedUpdateCallbacks = new Queue<Action> ();

	private void Update () {
		while (updateCallbacks.Count > 0) {
			updateCallbacks.Dequeue () ();
		}
	}

	private void FixedUpdate () {
		while (fixedUpdateCallbacks.Count > 0) {
			fixedUpdateCallbacks.Dequeue () ();
		}
	}

	/// <summary>
	/// Schedule the Action to be run in the next Update call
	/// </summary>
	public static void ExecuteOnUpdate (Action action) {
		if (action != null) {
			Instance.updateCallbacks.Enqueue (action);
		}
	}

	/// <summary>
	/// Schedule the Action to be run in the next FixedUpdate call
	/// </summary>
	/// <param name="action"></param>
	public static void ExecuteOnFixedUpdate (Action action) {
		if (action != null) {
			Instance.fixedUpdateCallbacks.Enqueue (action);
		}
	}
}
