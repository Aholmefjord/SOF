/// By: Tom Kueh
using UnityEngine;

/// <summary>
/// Tom: mainly to be used in conjunction with [RequireComponent] attribute; 
/// To get an attach object on a monobehaviour
/// </summary>
/// <typeparam name="T"></typeparam>
public class QuickComponentAccess<T> {

    /// <summary>
    /// the parent gameobject to get component from
    /// </summary>
    private GameObject _refObject;
    /// <summary>
    /// the component object
    /// </summary>
    private T _value;

    #region Ctor
    public QuickComponentAccess() {
        _refObject = null;
        _value = default(T);
    }
    public QuickComponentAccess(GameObject parent) {
        if (parent == null)
            return;
        ReferenceObject = parent;
        T temp = Value;
    }
    #endregion

    /// <summary>
    /// the parent gameobject to get the component from
    /// </summary>
    public GameObject ReferenceObject {
        set {
            if (value == null)
                return;
            if(_refObject != value) {
                _value = default(T);
                _refObject = value;
            }
        }
    }
    /// <summary>
    /// the component of type T, provided a Gameobject has been set;
    /// See QuickComponentAccess::ReferenceObject
    /// </summary>
    public T Value {
        get {
            if(_value == null)
                _value = _refObject.GetComponent<T>();
            return _value;
        }
    }

    /// <summary>
    /// get the component on a given object, and cache it in this instance
    /// </summary>
    /// <param name="parentObj"></param>
    /// <returns></returns>
    public T ValueByRef(GameObject parentObj)
    {
        ReferenceObject = parentObj;
        return Value;
    }

    public override string ToString()
    {
        return string.Format("QuickComponentAccess<{1}> gameobject = {0}", _refObject, typeof(T));
    }
}
