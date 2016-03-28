using UnityEngine;
using System.Collections;

public class DontDestroyMe : MonoBehaviour {
	private static bool _exists = false;

	void Awake () {
		if (_exists)
			Destroy (gameObject);
		_exists = true;
		DontDestroyOnLoad (gameObject);
	}
}
