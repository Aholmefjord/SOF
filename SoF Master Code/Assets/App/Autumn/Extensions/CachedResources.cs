///
/// Cached Resources
/// A utility script that help load GameObject prefabs from Resources and cache it for multiple access
/// Provide 2 basic methods:
/// - Spawn method to initialize the GameObject from a Resources prefab (with or without using the Object Pool)
/// - Load method to just load and get the Resources prefab
/// Require ObjectPoolSystem script to work
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JULESTech.Resources; // assetbundles

public static class CachedResources {
	private static Dictionary<Type, Dictionary<string, object>> PrefabDictionary = new Dictionary<Type, Dictionary<string, object>> ();

	public static GameObject Spawn (string resourcePath, bool usingObjectPool = true, int initPoolSize = 20) {
		// Load the prefab from Resources and cache to the PrefabDictionary
		GameObject prefab = Load<GameObject> (resourcePath);

		// Return null if cannot load the prefab
		if (prefab == null) {
			return null;
		}

		// Spawn the prefab either by using object pool or normal Instantiate
		GameObject instance;
		if (usingObjectPool) {
			// This will create a object pool for this prefab
			// Internally, it will check and skip if the pool is already created
			prefab.CreatePool (initPoolSize);
			instance = prefab.Spawn ();
		} else {
			instance = GameObject.Instantiate (prefab);
		}

		return instance;
    }
    public static GameObject Spawn(GameObject prefab, bool usingObjectPool = true, int initPoolSize = 20)
    {
        // Load the prefab from Resources and cache to the PrefabDictionary
        //GameObject prefab = Load<GameObject>(resourcePath);

        // Return null if cannot load the prefab
        if (prefab == null) {
            return null;
        }

        // Spawn the prefab either by using object pool or normal Instantiate
        GameObject instance;
        if (usingObjectPool) {
            // This will create a object pool for this prefab
            // Internally, it will check and skip if the pool is already created
            prefab.CreatePool(initPoolSize);
            instance = prefab.Spawn();
        } else {
            instance = GameObject.Instantiate(prefab);
        }

        return instance;
    }

	public static T Load<T> (string resourcePath) where T : UnityEngine.Object {
		// Load the prefab from Resources and cache to the PrefabDictionary
		if (!PrefabDictionary.ContainsKey (typeof (T))) {
			PrefabDictionary.Add (typeof (T), new Dictionary<string, object> ());
		}
		var prefabs = PrefabDictionary[typeof (T)];

		object prefab = null;

		if (!prefabs.TryGetValue (resourcePath, out prefab)) {
			prefab = Resources.Load<T> (resourcePath);
			if (prefab != null) {
				prefabs.Add (resourcePath, prefab);
			}
		}
		return (T) prefab;
	}

    static Dictionary<string, GameObject> bundlePrefabCache = new Dictionary<string, GameObject>();
    /// <summary>
    /// load source from assetbundle, create an instance from that and return that as result;
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="resource"></param>
    /// <param name="on_complete"></param>
    public static void Spawn(string assetBundleName, string resource, System.Action<GameObject> on_complete)
    {
        try {
            AssetBundleManager.Instance.StartCoroutine(_spawnInner(assetBundleName, resource, on_complete));
        }catch(ArgumentException) {
            Debug.LogErrorFormat("What is double loaded into dictionary: [{0}] from [{1}]", resource, assetBundleName);
        }
    }
    static IEnumerator _spawnInner(string bundle, string assetName, System.Action<GameObject> on_complete)
    {
        GameObject prefab = null;
        GameObject inst = null;
        string cacheKey = bundle + "/" + assetName;

        // load from cache if possible
        if (bundlePrefabCache.ContainsKey(cacheKey)) {
            // cache hit;
            prefab = bundlePrefabCache[cacheKey];
            inst = GameObject.Instantiate(prefab);
            if (on_complete != null)
                on_complete(inst);
            //Debug.Log("CacheResource: cache hit");
            yield break;
        }

        // cache missed, proceed to load from bundle
        //Debug.Log("CacheResource: start load asset");
        yield return AssetBundleManager.LoadAsset(bundle, assetName, (assetObj) => {
            //Debug.Log("CacheResource: continue load asset");

            // async problem: another load function could had been call on the same frame;
            if (bundlePrefabCache.ContainsKey(cacheKey)) {
                // cache hit;
                prefab = bundlePrefabCache[cacheKey];
                inst = GameObject.Instantiate(prefab);
                if (on_complete != null)
                    on_complete(inst);
            } else {

                if (assetObj != null) {
                    prefab = assetObj as GameObject;

                    // prefab successfully load, cache it
                    if (prefab != null) {
                        bundlePrefabCache.Add(cacheKey, prefab);
                        inst = GameObject.Instantiate(prefab);
                    }

                    if (on_complete != null) {
                        on_complete(inst);
                    }
                } else {
                    // failed to load
                    Debug.LogError("CacheResource: asset not load, missing " + bundle + "," + assetName);
                }
            }
        });
    }

	public static GameObject Load (string resourcePath) {
		return Load<GameObject> (resourcePath);
	}
}