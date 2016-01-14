using UnityEngine;
using System.Collections;

public class SlowMovementPlayerControllerModifier : DefaultPlayerControllerModifier {
	private float startTime;
	private IPlayerController _playerController;

	public SlowMovementPlayerControllerModifier(IPlayerController playerController) {
		startTime = Time.time;
		_playerController = playerController;
	}

	public override float speedScalar { 
		get { 
			if(!isRadioactive)
				return 0.5f; 
			return _playerController.speedScalar;
		} 
		set { _playerController.speedScalar = value; } 
	}

	public override bool isRadioactive { get { return startTime + duration >= Time.time; } }
}
