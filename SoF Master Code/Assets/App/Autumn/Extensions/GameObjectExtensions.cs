using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class GameObjectExtensions {

	public static RectTransform rectTransform (this GameObject gameObject) {
		return (RectTransform) gameObject.transform;
	}

	public static void AddEventTrigger (this GameObject gameObject, EventTriggerType type, UnityAction callback) {
		gameObject.ClearEventTrigger (type);
		EventTrigger trigger = gameObject.GetComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = type;
		entry.callback.AddListener ((eventData) => { if (callback != null) callback (); });
		trigger.triggers.Add (entry);
	}

	public static void ClearEventTrigger (this GameObject gameObject, EventTriggerType type) {
		EventTrigger trigger = gameObject.GetComponent<EventTrigger> ();
		if (trigger == null) {
			trigger = gameObject.AddComponent<EventTrigger> ();
		}
		foreach (var entry in trigger.triggers.ToArray ()) {
			if (entry.eventID == type) {
				trigger.triggers.Remove (entry);
			}
		}
	}

	public static void SetSprite (this GameObject gameObject, Sprite sprite) {
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		if (spriteRenderer != null) {
			spriteRenderer.sprite = sprite;
		}
	}

	/// <summary>
	/// Find a child (or grandchild) by name recursively under this Transform
	/// </summary>
	/// <param name="transform">The root of the transform to start finding</param>
	/// <param name="childName">The name of the child to find</param>
	/// <returns>The child's transform. Null if cannot find</returns>
	public static Transform FindChild (Transform transform, string childName) {
        Transform result = transform.Find (childName);
        if (result == null) {
            foreach (Transform child in transform) {
                result = FindChild (child, childName);
                if (result != null)
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// Find a child (or grandchild) by name recursively under this GameObject
    /// </summary>
    /// <param name="gameObject">The root of the game object to start finding</param>
    /// <param name="childName">The name of the child to find</param>
    /// <returns>The child's gameObject. Null if cannot find</returns>
    public static GameObject FindChild (this GameObject gameObject, string childName) {
        Transform transform = FindChild (gameObject.transform, childName);
        if (transform == null) {
            return null;
        } else {
            return transform.gameObject;
        }
    }

    /// <summary>
    /// Get the UI.Button component with the specific name recursively under this GameObject
    /// </summary>
    /// <param name="gameObject">The root of the game object to start finding</param>
    /// <param name="buttonName">The name of the button to find</param>
    /// <returns>The button component. Null if cannot find</returns>
    public static Button GetButton (this GameObject gameObject, string buttonName) {
        GameObject go = gameObject.FindChild (buttonName);
        if (go == null) {
            return null;
        } else {
            return go.GetComponent<Button> ();
        }
    }

    /// <summary>
    /// Set the content of the UI.Text component of this GameObject
    /// </summary>
    /// <param name="gameObject">The target GameObject</param>
    /// <param name="text">the content to set</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject SetText (this GameObject gameObject, string text) {
        Text textComp = gameObject.GetComponent<Text> ();
        if (textComp == null) {
            textComp = gameObject.GetComponentInChildren<Text> ();
        }
        textComp.text = text;
        return gameObject;
    }

#if SCRIPT_SYSTEM
	/// <summary>
	/// Set the content of the UI.Text component of this GameObject
	/// </summary>
	/// <param name="gameObject">The target GameObject</param>
	/// <param name="text">the content to set</param>
	/// <returns>The same GameObject sent in</returns>
	public static GameObject SetScriptText (this GameObject gameObject, string text) {
		return gameObject.SetText (ScriptSystem.ParseExpressionScript (text));
	}
#endif

	/// <summary>
	/// Set the content of the UI.Image component of this GameObject
	/// </summary>
	/// <param name="gameObject">The target GameObject</param>
	/// <param name="sprite">The content to set</param>
	/// <returns>The same GameObject sent in</returns>
	public static GameObject SetImage (this GameObject gameObject, Sprite sprite) {
        Image imageComp = gameObject.GetComponent<Image> ();
        if (imageComp == null) {
            imageComp = gameObject.GetComponentInChildren<Image> ();
        }
        imageComp.sprite = sprite;
        return gameObject;
    }

	/// <summary>
	/// Set the color of the UI.Graphic component of this GameObject
	/// </summary>
	/// <param name="gameObject">The target GameObject</param>
	/// <param name="color">The color to set</param>
	/// <returns>The same GameObject sent in</returns>
	public static GameObject SetColor (this GameObject gameObject, Color color) {
		Graphic graphicComp = gameObject.GetComponent<Graphic> ();
		if (graphicComp == null) {
			graphicComp = gameObject.GetComponentInChildren<Graphic> ();
		}
		graphicComp.color = color;
		return gameObject;
	}

	/// <summary>
	/// Set the alpha of the UI.Graphic component of this GameObject
	/// </summary>
	/// <param name="gameObject">The target GameObject</param>
	/// <param name="alphaa">The alpha to set</param>
	/// <returns>The same GameObject sent in</returns>
	public static GameObject SetAlpha (this GameObject gameObject, float alpha) {
		Graphic graphicComp = gameObject.GetComponent<Graphic> ();
		if (graphicComp == null) {
			graphicComp = gameObject.GetComponentInChildren<Graphic> ();
		}
		Color color = graphicComp.color;
		color.a = alpha;
		graphicComp.color = color;
		return gameObject;
	}

	/// <summary>
	/// Clear all OnClick subscriber of the UI.Button component of this GameObject
	/// </summary>
	/// <param name="gameObject">The target GameObject</param>
	/// <returns>The same GameObject sent in</returns>
	public static GameObject ClearOnClick (this GameObject gameObject) {
        Button button = gameObject.GetComponent<Button> ();
        if (button == null) {
            button = gameObject.GetComponentInChildren<Button> ();
        }
        button.onClick.RemoveAllListeners ();
        return gameObject;
    }
    /// <summary>
    /// Assign the callback for UI.Button's OnClick event of this GameObject
    /// </summary>
    /// <param name="gameObject">The target GameObject</param>
    /// <param name="callback">The callback to be called upon OnClick</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject OnClick (this GameObject gameObject, UnityAction callback) {
        Button button = gameObject.GetComponent<Button> ();
        if (button == null) {
            button = gameObject.GetComponentInChildren<Button> ();
        }
		gameObject.ClearOnClick ();
        button.onClick.AddListener (callback);
        return gameObject;
    }

    /// <summary>
    /// Assign the callback for UI.Toggle's OnToggle event of this GameObject
    /// </summary>
    /// <param name="gameObject">The target GameObject</param>
    /// <param name="callback">The callback to be called upon OnToggle</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject OnToggle (this GameObject gameObject, UnityAction<bool> callback) {
        Toggle toggle = gameObject.GetComponent<Toggle> ();
        toggle.onValueChanged.RemoveAllListeners ();
        toggle.onValueChanged.AddListener (callback);
        return gameObject;
    }

    /// <summary>
    /// Destroy all children under this GameObject
    /// </summary>
    /// <param name="gameObject">The target GameObject</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject Clear (this GameObject gameObject) {
        for (var i = 0; i < gameObject.transform.childCount; ++i) {
            Transform child = gameObject.transform.GetChild (i);
            GameObject.Destroy (child.gameObject);
        }
        return gameObject;
    }

    /// <summary>
    /// Get the Bounds of this GameObject
    /// </summary>
    /// <param name="gameObject">The target GameObject</param>
    /// <returns>The Bounds of this GameObject</returns>
    public static Bounds GetBounds (this GameObject gameObject) {
        return gameObject.transform.GetBounds ();
    }

    /// <summary>
    /// Get the Bounds of this Transform
    /// </summary>
    /// <param name="transform">The target transform</param>
    /// <returns>The Bounds of this Transform</returns>
    public static Bounds GetBounds (this Transform transform) {
        Bounds bounds = new Bounds ();
        if (transform.GetComponent<SpriteRenderer> () == null) {
            bounds.center = transform.position;
        } else {
            bounds = transform.GetComponent<SpriteRenderer> ().bounds;
        }
        foreach (Transform trans in transform) {
            bounds.Encapsulate (trans.gameObject.GetBounds ());
        }
        return bounds;
    }

    /// <summary>
    /// Set the layer for all children under this GameObject
    /// </summary>
    /// <param name="obj">The GameObject root</param>
    /// <param name="layerName">The name of the layer to set</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject SetLayerRecursively (this GameObject obj, string layerName) {
        int layer = LayerMask.NameToLayer (layerName);
        return obj.SetLayerRecursively (layer);
    }

    /// <summary>
    /// Set the layer for all children under this GameObject
    /// </summary>
    /// <param name="obj">The GameObject root</param>
    /// <param name="layer">The layer number to set</param>
    /// <returns>The same GameObject sent in</returns>
    public static GameObject SetLayerRecursively (this GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            child.gameObject.SetLayerRecursively (layer);
        }
        return obj;
    }

#if GEAnim
	public static GameObject GUIMoveOut (this GameObject gameObject) {
		bool hasAnimate = false;
		foreach (GEAnim anim in gameObject.GetComponentsInChildren<GEAnim> ()) {
			anim.MoveOut ();
			hasAnimate = true;
		}
		if (!hasAnimate) {
			gameObject.SetActive (false);
		}
		return gameObject;
	}
	public static GameObject GUIMoveIn (this GameObject gameObject) {
		if (!gameObject.activeInHierarchy) {
			gameObject.SetActive (true);
		}
		foreach (GEAnim anim in gameObject.GetComponentsInChildren<GEAnim> ()) {
			anim.MoveIn ();
		}
		return gameObject;
	}
#endif
}