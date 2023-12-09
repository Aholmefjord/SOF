using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// helper proccessor for SetupUI calls, caching the FindObject/FindChild results
/// Tom: making those SetupUI calls abit more efficient, too lazy to change all the,
/// </summary>
public sealed class MultiLanguageApplicator {
    GameObject m_Root;
    Dictionary<string, GameObject> m_Cache; // full path as key, object as value

    public MultiLanguageApplicator(GameObject root)
    {
        if (root == null) {
            Debug.LogError("not valid root");
            return;
        }

        m_Root = root;
        m_Cache = new Dictionary<string, GameObject>();
    }
    MultiLanguageApplicator() { }

    /// <summary>
    /// MultiLanguage.apply()
    /// </summary>
    /// <param name="hierachyPath">'/' seperated path, each processed into nodes; 
    /// for each seperated node, FindChild will be called recursively to get a child by that node, 
    /// then proccess the next node until all has been processed.
    /// The result of the last node is used to supply MultiLanguage.apply()
    /// </param>
    /// <param name="name">name id loaded in MultiLanguage.cs</param>
    public void ApplyText(string hierachyPath, string name)
    {
        if (m_Cache.ContainsKey(hierachyPath)) {
            MultiLanguage.getInstance().apply(m_Cache[hierachyPath], name);
            return;
        }
        string[] brokenPath = hierachyPath.Split('/');
        GameObject obj = RecursiveFindChild(m_Root, brokenPath, 0);

        if (obj != null) {
            MultiLanguage.getInstance().apply(obj, name);
        }
    }
    /// <summary>
    /// MultiLanguage.applyImage()
    /// </summary>
    /// <param name="hierachyPath">see ApplyText</param>
    /// <param name="name"></param>
    public void ApplyImage(string hierachyPath, string name)
    {
        if (m_Cache.ContainsKey(hierachyPath)) {
            MultiLanguage.getInstance().applyImage(m_Cache[hierachyPath].GetComponent<UnityEngine.UI.Image>(), name);
            return;
        }
        string[] brokenPath = hierachyPath.Split('/');
        GameObject obj = RecursiveFindChild(m_Root, brokenPath, 0);

        if (obj != null) {
            MultiLanguage.getInstance().applyImage(obj.GetComponent<UnityEngine.UI.Image>(), name);
        }
    }
    /// <summary>
    /// generic application
    /// </summary>
    /// <param name="hierachyPath"></param>
    /// <param name="function"></param>
    public void ExecuteOn(string hierachyPath, Action<GameObject> function)
    {
        if (m_Cache.ContainsKey(hierachyPath)) {
            if (function != null) function(m_Cache[hierachyPath]);
            return;
        }
        string[] brokenPath = hierachyPath.Split('/');
        GameObject obj = RecursiveFindChild(m_Root, brokenPath, 0);

        if (obj != null && function != null) {
            function(obj);
        }
    }
    /// <summary>
    /// caches any child at every level
    /// NOTE: GameObject.FindChild() is a custom GameObjectExtension
    /// may want to rewrite the GetChild
    /// </summary>
    /// <param name="refObject"></param>
    /// <param name="path"></param>
    /// <param name="curr_depth"></param>
    /// <returns></returns>
    GameObject RecursiveFindChild(GameObject refObject, string[] path, int curr_depth)
    {
        if (path.Length == curr_depth) {
            return refObject;
        }
        System.Text.StringBuilder build = new System.Text.StringBuilder();  // to build up split path
        for (int i = 0; i <= curr_depth; ++i) {
            build.Append(path[i]);
            if (i != curr_depth)
                build.Append('/');
        }
        GameObject result = null;
        bool cacheHit = false;
        // looking for child gameobject
        if (m_Cache.ContainsKey(build.ToString())) {
            // cache hit
            result = m_Cache[build.ToString()];
            cacheHit = true;
            //Debug.Log("CACHE HIT: " + path[curr_depth]);
        } else {
            // cache missed, findchild now
            result = refObject.FindChild(path[curr_depth]);
            if (result != null) {
            } else {
                Debug.LogError(string.Format("[{0}] is not be found under [{1}]", path[curr_depth], refObject));
                return null;
            }
        }

        if (cacheHit == false) {
            // cache result since it was not found in cache
            m_Cache.Add(build.ToString(), result);
        }
        return RecursiveFindChild(result, path, curr_depth + 1);
    }
}