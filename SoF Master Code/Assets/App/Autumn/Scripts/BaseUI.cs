using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

using DG.Tweening;

public class BaseUI : MonoBehaviour {
    private static GameObject _uiRoot;
    public static GameObject UIRoot {
        get {
            if (_uiRoot == null) {
                _uiRoot = GameObject.FindGameObjectWithTag ("UIRoot");
            }
            if (_uiRoot == null) {
                _uiRoot = new GameObject ("UIScreens");
                _uiRoot.tag = "UIRoot";
            }
            return _uiRoot;
        }
    }


    protected UIManager uiManager;
    protected CanvasGroup canvasGroup;
    protected bool closing = false;
    protected virtual Sequence TweenIn { get { return null; } }
    protected virtual Sequence TweenOut { get { return null; } }

    public virtual void Close () {
        if (closing) {
            return;
        }

        closing = true;
        enabled = false;
        canvasGroup.interactable = false;
        if (TweenOut == null) {
            Destroy (gameObject);
        } else {
            TweenOut.OnComplete (() => Destroy (gameObject)).SetUpdate (true).Play ();
        }
    }

    public virtual void CloseImmediately () {
        Destroy (gameObject);
    }

    protected virtual void Start () {
        uiManager = new UIManager (gameObject);

        canvasGroup = GetComponent<CanvasGroup> ();
        if (canvasGroup == null) {
            foreach (Transform child in transform) {
                canvasGroup = child.GetComponent<CanvasGroup> ();
                if (canvasGroup != null) continue;
            }
        }
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup> ();
        }

        // Tween in sequence
        TweenIn.SetUpdate (true).Play ();
    }
}
