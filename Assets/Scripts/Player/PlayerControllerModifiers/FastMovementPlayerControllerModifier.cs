using UnityEngine;
using System.Collections;

public class FastMovementPlayerControllerModifier : DefaultPlayerControllerModifier {
	public FastMovementPlayerControllerModifier(IPlayerController playerController) {
		_startTime = Time.time;
		_playerController = playerController;
	}

	public override float speedScalar { 
		get { 
			if(isRadioactive)
				return 4.0f; 
			return _playerController.speedScalar;
		} 
		set { _playerController.speedScalar = value; } 
	}
}
