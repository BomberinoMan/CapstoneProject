using UnityEngine;
using System.Collections;

public class ReverseMovementPlayerControllerModifier : DefaultPlayerControllerModifier {
	private float startTime;
	private IPlayerController _playerController;

	public ReverseMovementPlayerControllerModifier(IPlayerController playerController) {
		startTime = Time.time;
		_playerController = playerController;
	}

	public override bool reverseMovement { 
		get { 
			return isRadioactive;
		} 
		set { _playerController.reverseMovement = value; } 
	}

	public override bool isRadioactive { get { return startTime + duration >= Time.time; } }
}
