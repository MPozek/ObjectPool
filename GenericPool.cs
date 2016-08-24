using UnityEngine;
using System.Collections.Generic;

public abstract class GenericPool : MonoBehaviour {

	Dictionary<int,Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>> ();

	static GenericPool _instance;

	public static GenericPool instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<GenericPool> ();
			}
			return _instance;
		}
	}

	public void CreatePool(GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID ();

		if (!poolDictionary.ContainsKey (poolKey)) {
			poolDictionary.Add (poolKey, new Queue<ObjectInstance> ());

			GameObject poolHolder = new GameObject (prefab.name + " pool");
			poolHolder.transform.parent = transform;

			for (int i = 0; i < poolSize; i++) {
				ObjectInstance newObject = new ObjectInstance(Instantiate (prefab) as GameObject);
				poolDictionary [poolKey].Enqueue (newObject);
				newObject.SetParent (poolHolder.transform);
			}
		}
	}

	public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation) {
		int poolKey = prefab.GetInstanceID ();

		if (poolDictionary.ContainsKey (poolKey)) {
			ObjectInstance objectToReuse = poolDictionary [poolKey].Dequeue ();
			poolDictionary [poolKey].Enqueue (objectToReuse);

			objectToReuse.Reuse (position, rotation);
		}
	}

	public class ObjectInstance {

		GameObject gameObject;
		Transform transform;

		bool hasPoolObjectComponent;
		IPoolObject poolObjectScript;

		public ObjectInstance(GameObject objectInstance) {
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive(false);

			if (gameObject.GetComponent<IPoolObject>()) {
				hasPoolObjectComponent = true;
				poolObjectScript = gameObject.GetComponent<IPoolObject>();
			}
		}

		public void Reuse(Vector3 position, Quaternion rotation) {
			gameObject.SetActive (true);
			transform.position = position;
			transform.rotation = rotation;

			if (hasPoolObjectComponent) {
				poolObjectScript.OnObjectReuse ();
			}
		}

		public void SetParent(Transform parent) {
			transform.parent = parent;
		}
	}
}
