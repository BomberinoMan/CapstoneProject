using UnityEngine;
using System.Collections;

public class PlayerControllerComponent : MonoBehaviour {
	private IPlayerController _playerController;

	//TODO Need to create this as a layer above PlayerController
	//		- Need to re-reference everything that is modifying parameters in this class
	//		- Need to pass everything that is needed down to the actual controller

	void Start () {
	
	}
	
	void Update () {
	
	}
}
