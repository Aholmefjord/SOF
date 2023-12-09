using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TextRiser {

    private static UIManager _canvasUI;
    /*
    private static UIManager CanvasUI {
        get {
            if (_canvasUI == null) {
                //var _canvas = CachedResources.Spawn ("ui/Text Riser Canvas", false);

                JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.TUMBLE_TROUBLE_SHARED, "Text Riser Canvas", (loadedAsset)=> {
                    var _canvas = loadedAsset as GameObject;
                    _canvas = GameObject.Instantiate(_canvas);
                    Object.DontDestroyOnLoad(_canvas);
                    // _canvas.hideFlags = HideFlags.HideInHierarchy || HideFlags.HideInInspector;
                    _canvasUI = new UIManager(_canvas);
                });
            }
            return _canvasUI;
        }
    }
    //*/
    private static ObjectInitHelper processor = null;

    public static void Create (string type, string content, Vector3 screenPosition, Sequence sequence = null)
    {
        if (processor == null) {
            processor = new ObjectInitHelper(content, screenPosition, sequence);
        } else {
            processor.Set(content, screenPosition, sequence);
        }
        if (_canvasUI == null) {
            //var _canvas = CachedResources.Spawn ("ui/Text Riser Canvas", false);
            JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.TUMBLE_TROUBLE_SHARED, "Text Riser Canvas", (loadedAsset) => {
                GameObject _canvas = GameObject.Instantiate(loadedAsset as GameObject);
                Object.DontDestroyOnLoad(_canvas);

                _canvasUI = new UIManager(_canvas);
                processor.Do(_canvasUI.CreateFromTemplate(type));
            });
        } else {
            processor.Do(_canvasUI.CreateFromTemplate(type));
        }
    }
    /// <summary>
    /// Tom: I know this method is abit convoluted and not memory efficient, 
    /// </summary>
    class ObjectInitHelper {
        string m_content;
        Vector3 m_pos;
        Sequence m_seq;
        public ObjectInitHelper(string content, Vector3 pos, Sequence seq)
        {
            Set(content, pos, seq);
        }
        public void Set(string content, Vector3 pos, Sequence seq)
        {
            m_content = content;
            m_pos = pos;
            m_seq = seq;
        }
        public void Do(GameObject obj)
        {
            obj.SetText(m_content);
            obj.transform.position = m_pos;
            if (m_seq == null) {
                /*sequence = DOTween.Sequence()
                    .Append(go.transform.DOLocalMoveY(200f, 3f).SetRelative(true))
                    .Join(go.transform.GetComponent<Text>().DOFade(0f, 3f));*/
                m_seq = DOTween.Sequence()
                .Append(obj.transform.DOLocalMoveY(100f, 0.5f).SetRelative(true))
                .Append(obj.transform.GetComponent<Text>().DOFade(0f, 0.5f));
            }
            m_seq.OnComplete(() => Object.Destroy(obj));
        }
    }
}
