using UnityEngine;
using UnityEditor;

namespace JULESTech.Resources {
    public class BundleCacheClean {

        [MenuItem("AssetBundle/Clear Unity Cache", priority = 3)]
        public static void CleanUnityCache()
        {
            if (Caching.CleanCache()) {
                Debug.Log("[Clear Unity Cache] cache cleared");
            } else {
                Debug.LogError("[Clear Unity Cache] cache is being used, cache clear failed");
            }
        }
    }
}