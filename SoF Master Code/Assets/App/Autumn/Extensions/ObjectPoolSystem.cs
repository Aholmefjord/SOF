/// 
/// Object Pool System
/// ngtrhieu
/// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Object Pool Manager manages all instances of different prefabs in-game to help spawn and recycle them
/// </summary>
public sealed class ObjectPoolManager : MonoBehaviour {

	#region --- Declaration ---

	private static bool isApplicationShutDown = false;
	public class ShutDownNotifier : MonoBehaviour {
		private void OnApplicationQuit () {
			isApplicationShutDown = true;
		}	
	}

	private static ShutDownNotifier _shutDownNotifier;
	private static ObjectPoolManager _instance;
	private static ObjectPoolManager Instance {
		get {
			if (_shutDownNotifier == null) {
				GameObject go = new GameObject ("Shut Down Notifier");
				go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
				_shutDownNotifier = go.AddComponent<ShutDownNotifier> ();
			}
			if (_instance == null && !isApplicationShutDown) {
				GameObject go = new GameObject ("Object Pool System");
				_instance = go.AddComponent<ObjectPoolManager> ();
			}
			return _instance;
		}
	}

	/// <summary>
	/// Map the prefab (key) to the list of available pooled game object (value)
	/// </summary>
	private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>> ();
	/// <summary>
	/// The spawned game object (key) to its prefab origin (value)
	/// </summary>
	private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject> ();
	#endregion



	#region --- Singleton ---
	/// <summary>
	/// Private constructor - prevent initialization for other sources
	/// </summary>
	private ObjectPoolManager () { }

	private IEnumerator DelayRecycle (GameObject obj, float delay) {
		yield return new WaitForSeconds (delay);		
		RecycleGameObject (obj);
	}
	#endregion



	#region --- Public Static Methods ---

	/// <summary>
	/// Create a pool from the Game Object this Component is attaching on.
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="initialPoolSize">The size of the pool to be initialized</param>
	public static void CreatePool<T>(T component, int initialPoolSize) where T : Component {
		// If the Create Pool is called on component T, just create with its attaching game object
		CreatePool (component.gameObject, initialPoolSize);
	}

	/// <summary>
	/// Create a pool from the specific Game Object
	/// </summary>
	/// <param name="prefab">The original Game Object to create the pool from</param>
	/// <param name="initialPoolSize">THe size of the pool to be initialized</param>
	public static void CreatePool (GameObject prefab, int initialPoolSize) {
		// Ensure that we have not created the pool with this prefab before
		if (prefab != null && !Instance.pooledObjects.ContainsKey (prefab)) {
			// Create an list of available pooled object that are in the game (initially empty)
			List<GameObject> list = new List<GameObject> ();

			// Map the prefab to the new list
			Instance.pooledObjects.Add (prefab, list);

			// Start filling up instances to match the initial size of the pool
			if (initialPoolSize > 0) {
				// Spawn all pooled objects under the Manager
				Transform parent = Instance.transform;

				// Retrieve the active state of the prefab and disable it
				// This is to make sure all pooled object spawned are in disable mode
				// to ensure no script under it would be run
				bool active = prefab.activeSelf;
				prefab.SetActive (false);

				// Start instantiate objects and add to list
				while (list.Count < initialPoolSize) {
					GameObject obj = (GameObject) Instantiate (prefab);
					obj.name = prefab.name;
					obj.transform.SetParent (parent);
					list.Add (obj);
				}

				// Restore the previous active state of the prefab
				prefab.SetActive (active);
			}
		}
	}

	/*********************
	 * Various generic shortcut methods of Spawn, return Component
	 ********************/
	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result,
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component) where T : Component {
		return Spawn (component.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T> ();
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component, Transform parent, Vector3 position, Quaternion rotation) where T : Component {
		return Spawn (component.gameObject, parent, position, rotation).GetComponent<T> ();
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component, Vector3 position, Quaternion rotation) where T : Component {
		return Spawn (component.gameObject, null, position, rotation).GetComponent<T> ();
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component, Transform parent, Vector3 position) where T : Component {
		return Spawn (component.gameObject, parent, position, Quaternion.identity).GetComponent<T> ();
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component, Vector3 position) where T : Component {
		return Spawn (component.gameObject, null, position, Quaternion.identity).GetComponent<T> ();
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(T component, Transform parent) where T : Component {
		return Spawn (component.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T> ();
	}

	/*********************
	 * Various shortcut methods of Spawn, return GameObject
	 ********************/
	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab, Transform parent, Vector3 position) {
		return Spawn (prefab, parent, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab, Vector3 position, Quaternion rotation) {
		return Spawn (prefab, null, position, rotation);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab, Transform parent) {
		return Spawn (prefab, parent, Vector3.zero, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab, Vector3 position) {
		return Spawn (prefab, null, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab) {
		return Spawn (prefab, null, Vector3.zero, Quaternion.identity);
	}

	/*********************
	 * Implementation of Spawn 
	 ********************/
	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
		// The list of pooled object that mapped to this prefab
		List<GameObject> list;

		// The GameObject to return
		GameObject obj;

		if (Instance.pooledObjects.TryGetValue (prefab, out list)) {
			// This prefab has an object pool mapped to it
			// Now try to find an available pooled object
			obj = null;
			if (list.Count > 0) {
				// Try grab the first non-null object in the list
				// Remove all null objects along the way
				while (obj == null && list.Count > 0) {
					obj = list[0];
					list.RemoveAt (0);
				}

				// Obj now should not be null, otherwise the list is already empty
				if (obj != null) {
					// Transfer this object to the list of spawned objects
					Instance.spawnedObjects.Add (obj, prefab);

					// Initialize its transform
					obj.transform.SetParent (parent);
					obj.transform.localPosition = position;
					obj.transform.localRotation = rotation;

					// Activate it - all scripts inside now must handle the setup through OnEnable ()
					obj.SetActive (true);
				}
			}

			if (obj == null) {
				// The pool is empty - Just Instantiate and add to the spawned list

				// Instantiate a new object
				obj = (GameObject) Instantiate (prefab);
				obj.transform.SetParent (parent);
				obj.transform.localPosition = position;
				obj.transform.localRotation = rotation;

				// Add to the spawned list
				Instance.spawnedObjects.Add (obj, prefab);

				// Make sure it is active
				obj.SetActive (true);
			}
		} else {
			// No pool has been created for this object - Just Instantiate it normally
			obj = (GameObject) Instantiate (prefab, position, rotation);
			obj.transform.SetParent (parent);
		}

		return obj;
	}

	/// <summary>
	/// Recycle the Game Object this Component is attaching on. If the attaching Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="delay">The delay until recycle</param> 
	public static void Recycle<T>(T component, float delay = 0F) where T : Component {
		RecycleGameObject (component.gameObject, delay);
	}

	/// <summary>
	/// Recycle this Game Object. If this Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <param name="obj">The Game Object to be recycled</param>
	/// <param name="delay">The delay until recycle</param>
	public static void RecycleGameObject (GameObject obj, float delay = 0F) {
		if (obj == null) {
			return;
		}

		if (delay > 0F) {
			Instance.StartCoroutine (Instance.DelayRecycle (obj, delay));
		} else {
			GameObject prefab;

			// Check whether this game object was spawned from a pool

			// HACK: The first condition Instance != null is to check whether the game is stopping
			// If the game is stopping and somehow this script is destroyed first, then the Instance would be null
			// If that the case, then just safely Destroy the object, regardless whether it is a pool object or not

			if (Instance != null && Instance.spawnedObjects.TryGetValue (obj, out prefab)) {
				// This was a pool object
				// Just put it back to the pool
				Instance.pooledObjects[prefab].Add (obj);
				Instance.spawnedObjects.Remove (obj);

				// Also set it back under the Manager
				obj.transform.SetParent (Instance.transform);

				// Disable the GameObject - All scripts should handle cleaning up OnDisable ()
				obj.SetActive (false);

			} else {
                // This is not a pool object, let go through the childrens of the object and check whether there are
                // any pool object inside
                // If the object has no child, manually destroy it
                if (obj.transform.childCount == 0) {
                    Destroy (obj);
                } else {
                    foreach (Transform child in obj.transform) {
                        RecycleGameObject (child.gameObject);
                    }
                }
			}
		}
	}

	/// <summary>
	/// Recycle all Game Objects that are spawned from the same pool with the Game Object this Component is attaching on. If the attaching Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="delay">The delay until destruction</param>
	public static void RecycleAll<T>(T component, float delay = 0F) where T : Component {
		RecycleAllGameObjects (component.gameObject, delay);
	}

	/// <summary>
	/// Recycle all Game Objects that are spawned from the same pool with this Game Object. If this Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <param name="prefab">The Game Object to be recycled</param>
	/// <param name="delay">The delay until destruction</param>
	public static void RecycleAllGameObjects (GameObject prefab, float delay = 0F) {
		if (Instance != null) {
			List<GameObject> tempList = new List<GameObject> ();

			// Collect all copies of the specified prefab
			foreach (KeyValuePair<GameObject, GameObject> item in Instance.spawnedObjects) {
				if (item.Value == prefab) {
					tempList.Add (item.Key);
				}
			}

			// Go through all copies in the list and recycle them 
			foreach (GameObject o in tempList.ToArray ()) {
				RecycleGameObject (o, delay);
			}
		}
	}

	/// <summary>
	/// Recycle all spawned Game Objects that are in the scene
	/// </summary>
	/// <param name="delay">The delay until destruction</param>
	public static void RecycleAll (float delay = 0F) {
		// Get all pooled objects that are in the scene
		List<GameObject> tempList = new List<GameObject> (Instance.spawnedObjects.Keys);

		// Recycle them one by one
		foreach (GameObject o in tempList.ToArray ()) {
			RecycleGameObject (o, delay);
		}
	}

	/// <summary>
	/// Check whether this specific Game Object is spawned
	/// </summary>
	/// <param name="obj">The Game Object to check</param>
	/// <returns>True if this Object is spawned and active. False if this Game Object is not in any pool or it is in a pool but not being used</returns>
	public static bool IsSpawned (GameObject obj) {
		return Instance.spawnedObjects.ContainsKey (obj);
	}

	/// <summary>
	/// Count the number of unused copies of the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the Game Object</param>
	/// <returns>The number of unused copies that from the same pool with the Game Object</returns>
	public static int CountPooled<T>(T component) where T : Component {
		return CountPooled (component.gameObject);
	}

	/// <summary>
	/// Count the number of unused copies of this Game Object
	/// </summary>
	/// <param name="prefab">The target Game Object</param>
	/// <returns>The number of unused copies that from the same pool with the Game Object</returns>
	public static int CountPooled (GameObject prefab) {
		List<GameObject> list;
		if (Instance.pooledObjects.TryGetValue (prefab, out list)) {
			return list.Count;
		}
		return 0;
	}

	/// <summary>
	/// Count the number of using copies of the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to the attaching Game Object</param>
	/// <returns>The number of copies that is being used in the scene from the same pool with the Game Object</returns>
	public static int CountSpawned<T>(T component) where T : Component {
		return CountSpawned (component.gameObject);
	}

	/// <summary>
	/// Count the number of using copies of this Game Object
	/// </summary>
	/// <param name="prefab">The target Game Object</param>
	/// <returns>The number of copies that is being used in the scene from the same pool with the Game Object</returns>
	public static int CountSpawned (GameObject prefab) {
		int count = 0;
		foreach (GameObject instancePrefab in Instance.spawnedObjects.Values) {
			if (prefab == instancePrefab) {
				++count;
			}
		}
		return count;
	}

	/// <summary>
	/// Count the number of unused copies across all pools
	/// </summary>
	/// <returns>The number of unused copies across all pools</returns>
	public static int CountAllPooled () {
		int count = 0;
		foreach (List<GameObject> list in Instance.pooledObjects.Values) {
			count += list.Count;
		}
		return count;
	}

	/// <summary>
	/// Destroy all unused Game Objects from the pool
	/// </summary>
	/// <param name="prefab">The Game Object to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyPooledGameObjects (GameObject prefab, float delay = 0F) {
		if (Instance != null) {
			List<GameObject> pooled;
			if (Instance.pooledObjects.TryGetValue (prefab, out pooled)) {
				for (int i = 0; i < pooled.Count; ++i) {
					Destroy (pooled[i], 0F);
				}
				pooled.Clear ();
			}
		}
	}

	/// <summary>
	/// Destroy all unused Game Objects from the pool
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyPooled<T>(T component, float delay = 0F) where T : Component {
		DestroyPooledGameObjects (component.gameObject, delay);
	}

	/// <summary>
	/// Destroy all Game Objects, both unused or being used, from the pool
	/// </summary>
	/// <param name="prefab">The Game Object to get the pool</param>
	public static void DestroyAll (GameObject prefab) {
		RecycleAllGameObjects (prefab);
		DestroyPooledGameObjects (prefab);
	}

	/// <summary>
	/// Destroy all Game Objects, both unused or being used, from the pool
	/// </summary>
	/// <typeparam name="T">Component </typeparam>
	/// <param name="component">The Component to get the pool</param>
	public static void DestroyAll<T>(T component) where T : Component {
		DestroyAll (component.gameObject);
	}

	/// <summary>
	/// Get the list of Game Objects that were used to create pools
	/// </summary>
	/// <returns>The last of Game Objects that were used to create pools</returns>
	public static List<GameObject> GetAllPrefabs () {
		return new List<GameObject> (Instance.pooledObjects.Keys);
	}

	/// <summary>
	/// Get the list of unused Game Objects that were spawned from this prefab
	/// </summary>
	/// <param name="prefab">The original Game Object that was used to spawn the pool</param>
	/// <returns>The list of all unused Game Objects that were spawned from this prefab. If no pool was created from the prefab, null is returned</returns>
	public static List<GameObject> GetPooled (GameObject prefab) {
		List<GameObject> list;
		if (Instance.pooledObjects.TryGetValue (prefab, out list)) {
			return list;
		}
		return null;
	}

	/// <summary>
	/// Get the list of unused Game Objects that were spawned from the Game Object that this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The list of all unused Game Objects that were spawned from the attaching Game Object. If no pool was created from that Game Object, null is returned</returns>
	public static List<GameObject> GetPooled<T>(T component) where T : Component {
		return GetPooled (component.gameObject);
	}

	/// <summary>
	/// Get the list of objects that were spawned from this prefab and are being used in the scene
	/// </summary>
	/// <param name="prefab">The original Game Object that was used to spawn the pool</param>
	/// <returns>The list of objects that were spawned from this prefab and are being used in the scene. If no pool was created from the prefab, null is returned</returns>
	public static List<GameObject> GetSpawned (GameObject prefab) {
		if (Instance.pooledObjects.ContainsKey (prefab)) {
			List<GameObject> list = new List<GameObject> ();
			foreach (GameObject spawned in Instance.spawnedObjects.Keys) {
				GameObject go;
				if (Instance.spawnedObjects.TryGetValue (spawned, out go)) {
					if (go == prefab) {
						list.Add (spawned);
					}
				}
			}
			return list;
		}
		return null;
	}

	/// <summary>
	/// Get the list of objects that are being used in the scene and were spawned from the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The list of object that are being used in the scene and were spawned from the attaching Game Object. If no pool was created from the Game Object, null is return</returns>
	public static List<GameObject> GetSpawned<T>(T component) where T : Component {
		return GetSpawned (component.gameObject);
	}

	/// <summary>
	/// Get the list of objects that were spawned from this prefab
	/// </summary>
	/// <param name="prefab">The original Game Object that was used to spawn the pool</param>
	/// <returns>The list of objects that were spawned from this prefab. If no pool was created from the prefab, null is returned</returns>
	public static List<GameObject> GetAll (GameObject prefab) {
		if (Instance.pooledObjects.ContainsKey (prefab)) {
			List<GameObject> allPooled = GetPooled (prefab);
			List<GameObject> allSpawned = GetSpawned (prefab);
			List<GameObject> allItems = new List<GameObject> ();
			if (allPooled != null) {
				allItems.AddRange (allPooled);
			}
			if (allSpawned != null) {
				allItems.AddRange (allSpawned);
			}
			return allItems;
		}
		return null;

	}
	#endregion
}

/// <summary>
/// Object Pool Extension contains all extension methods, providing shortcuts to quickly access <see cref="ObjectPoolManager"/>'s static methods without access the class
/// </summary>
public static class ObjectPoolExtensions {

	/// <summary>
	/// Create a pool from the Game Object this Component is attaching on.
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="initialPoolSize">The size of the pool to be initialized</param>
	public static void CreatePool<T>(this T component, int initialPoolSize) where T : Component {
		ObjectPoolManager.CreatePool (component, initialPoolSize);
	}

	/// <summary>
	/// Create a pool from the specific Game Object
	/// </summary>
	/// <param name="prefab">The original Game Object to create the pool from</param>
	/// <param name="initialPoolSize">THe size of the pool to be initialized</param>
	public static void CreatePool (this GameObject prefab, int initialPoolSize) {
		ObjectPoolManager.CreatePool (prefab, initialPoolSize);
	}




	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component, Transform parent, Vector3 position, Quaternion rotation) where T : Component {
		return ObjectPoolManager.Spawn (component, parent, position, rotation);
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component, Vector3 position, Quaternion rotation) where T : Component {
		return ObjectPoolManager.Spawn (component, null, position, rotation);
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component, Transform parent, Vector3 position) where T : Component {
		return ObjectPoolManager.Spawn (component, parent, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component, Vector3 position) where T : Component {
		return ObjectPoolManager.Spawn (component, null, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component, Transform parent) where T : Component {
		return ObjectPoolManager.Spawn (component, parent, Vector3.zero, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object this Component is attaching on, returning the Component as the result,
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The Component from the new copy</returns>
	public static T Spawn<T>(this T component) where T : Component {
		return ObjectPoolManager.Spawn (component, null, Vector3.zero, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
		return ObjectPoolManager.Spawn (prefab, parent, position, rotation);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="position">The local position of the new copy</param>
	/// <param name="rotation">The rotation of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab, Vector3 position, Quaternion rotation) {
		return ObjectPoolManager.Spawn (prefab, null, position, rotation);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab, Transform parent, Vector3 position) {
		return ObjectPoolManager.Spawn (prefab, parent, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="position">The local position of the new copy</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab, Vector3 position) {
		return ObjectPoolManager.Spawn (prefab, null, position, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <param name="parent">The parent Transform that the new copy will be put under</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab, Transform parent) {
		return ObjectPoolManager.Spawn (prefab, parent, Vector3.zero, Quaternion.identity);
	}

	/// <summary>
	/// Spawn the copy of the Game Object, returning the new Game Object as the result
	/// </summary>
	/// <param name="prefab">The original Game Object to create the copy from</param>
	/// <returns>The newly created Game Object</returns>
	public static GameObject Spawn (this GameObject prefab) {
		return ObjectPoolManager.Spawn (prefab, null, Vector3.zero, Quaternion.identity);
	}


	/// <summary>
	/// Recycle the Game Object this Component is attaching on. If the attaching Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="delay">The delay until recycle</param>
	public static void Recycle<T>(this T component, float delay = 0F) where T : Component {
		ObjectPoolManager.Recycle (component, delay);
	}

	/// <summary>
	/// Recycle this Game Object. If this Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <param name="obj">The Game Object to be recycled</param>
	/// <param name="delay">The delay until recycle</param>
	public static void RecycleGameObject (this GameObject obj, float delay = 0F) {
		ObjectPoolManager.RecycleGameObject (obj, delay);
	}



	/// <summary>
	/// Recycle all Game Objects that are spawned from the same pool with the Game Object this Component is attaching on. If the attaching Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <param name="delay">The delay until recycle</param>
	public static void RecycleAll<T>(this T component, float delay = 0F) where T : Component {
		ObjectPoolManager.RecycleAll (component, delay);
	}

	/// <summary>
	/// Recycle all Game Objects that are spawned from the same pool with this Game Object. If this Game Object is not spawned from any pool, Destroy () is called instead
	/// </summary>
	/// <param name="prefab">The Game Object to be recycled</param>
	/// <param name="delay">The delay until recycle</param>
	public static void RecycleAll (this GameObject prefab, float delay = 0F) {
		ObjectPoolManager.RecycleAllGameObjects (prefab);
	}



	/// <summary>
	/// Count the number of unused copies of the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the Game Object</param>
	/// <returns>The number of unused copies that from the same pool with the Game Object</returns>
	public static int CountPooled<T>(this T component) where T : Component {
		return ObjectPoolManager.CountPooled (component);
	}

	/// <summary>
	/// Count the number of unused copies of this Game Object
	/// </summary>
	/// <param name="prefab">The target Game Object</param>
	/// <returns>The number of unused copies that from the same pool with the Game Object</returns>
	public static int CountPooled (this GameObject prefab) {
		return ObjectPoolManager.CountPooled (prefab);
	}



	/// <summary>
	/// Count the number of using copies of the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to the attaching Game Object</param>
	/// <returns>The number of copies that is being used in the scene from the same pool with the Game Object</returns>
	public static int CountSpawned<T>(this T component) where T : Component {
		return ObjectPoolManager.CountSpawned (component);
	}

	/// <summary>
	/// Count the number of using copies of this Game Object
	/// </summary>
	/// <param name="prefab">The target Game Object</param>
	/// <returns>The number of copies that is being used in the scene from the same pool with the Game Object</returns>
	public static int CountSpawned (this GameObject prefab) {
		return ObjectPoolManager.CountSpawned (prefab);
	}

	/// <summary>
	/// Get the list of objects that were spawned from this prefab
	/// </summary>
	/// <param name="prefab">The original Game Object that was used to spawn the pool</param>
	/// <returns>The list of objects that were spawned from this prefab. If no pool was created from the prefab, null is returned</returns>
	public static List<GameObject> GetSpawned (this GameObject prefab) {
		return ObjectPoolManager.GetSpawned (prefab);
	}

	/// <summary>
	/// Get the list of objects that are being used in the scene and were spawned from the Game Object this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The list of object that are being used in the scene and were spawned from the attaching Game Object. If no pool was created from the Game Object, null is return</returns>
	public static List<GameObject> GetSpawned<T>(this T component) where T : Component {
		return ObjectPoolManager.GetSpawned (component);
	}

	/// <summary>
	/// Get the list of unused Game Objects that were spawned from this prefab
	/// </summary>
	/// <param name="prefab">The original Game Object that was used to spawn the pool</param>
	/// <returns>The list of all unused Game Objects that were spawned from this prefab. If no pool was created from the prefab, null is returned</returns>
	public static List<GameObject> GetPooled (this GameObject prefab) {
		return ObjectPoolManager.GetPooled (prefab);
	}

	/// <summary>
	/// Get the list of unused Game Objects that were spawned from the Game Object that this Component is attaching
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the attaching Game Object</param>
	/// <returns>The list of all unused Game Objects that were spawned from the attaching Game Object. If no pool was created from that Game Object, null is returned</returns>
	public static List<GameObject> GetPooled<T>(this T component) where T : Component {
		return ObjectPoolManager.GetPooled (component);
	}

	/// <summary>
	/// Destroy all unused Game Objects from the pool
	/// </summary>
	/// <param name="prefab">The Game Object to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyPooled (this GameObject prefab, float delay = 0F) {
		ObjectPoolManager.DestroyPooledGameObjects (prefab, delay);
	}

	/// <summary>
	/// Destroy all unused Game Objects from the pool
	/// </summary>
	/// <typeparam name="T">Component</typeparam>
	/// <param name="component">The Component to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyPooled<T>(this T component, float delay = 0F) where T : Component {
		ObjectPoolManager.DestroyPooledGameObjects (component.gameObject, delay);
	}



	/// <summary>
	/// Destroy all Game Objects, both unused or being used, from the pool
	/// </summary>
	/// <param name="prefab">The Game Object to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyAll (this GameObject prefab, float delay = 0F) {
		ObjectPoolManager.DestroyAll (prefab);
	}

	/// <summary>
	/// Destroy all Game Objects, both unused or being used, from the pool
	/// </summary>
	/// <typeparam name="T">Component </typeparam>
	/// <param name="component">The Component to get the pool</param>
	/// <param name="delay">The delay until destruction</param>
	public static void DestroyAll<T>(this T component, float delay = 0F) where T : Component {
		ObjectPoolManager.DestroyAll (component.gameObject);
	}
}