using UnityEngine;
using System.Collections;

public interface IPoolObject {

	void OnObjectReuse ();

	void Destroy ();

}
