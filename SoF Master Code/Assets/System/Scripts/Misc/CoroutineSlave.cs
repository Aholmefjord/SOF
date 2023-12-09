using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton Coroutine slave. Allow coroutines even in non-MonoBehaviours
/// </summary>
public class CoroutineSlave : MonoBehaviour {
    static CoroutineSlave _inst;
    static CoroutineSlave Instance {
        get {
            if( _inst == null) {
                GameObject obj = new GameObject("Coroutine");
                obj.hideFlags = HideFlags.HideAndDontSave;
                _inst = obj.AddComponent<CoroutineSlave>();
                DontDestroyOnLoad(_inst);
            }
            return _inst;
        }
    }

    public static Coroutine Start(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine routine)
    {
        Instance.StopCoroutine(routine);
    }

    public static void Clear()
    {
        Instance.StopAllCoroutines();
    }
}
