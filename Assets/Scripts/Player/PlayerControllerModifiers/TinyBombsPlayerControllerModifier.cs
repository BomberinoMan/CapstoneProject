using UnityEngine;
using System.Collections;

public class TinyBombsPlayerControllerModifier : DefaultPlayerControllerModifier {
	private BombParams temp;

	public TinyBombsPlayerControllerModifier(IPlayerController playerController) {
		_startTime = Time.time;
		_playerController = playerController;
		temp = new BombParams ();

		temp.delayTime = _playerController.bombParams.delayTime;
		temp.explodingDuration = _playerController.bombParams.explodingDuration;
		temp.radius = 1;
		temp.warningTime = _playerController.bombParams.warningTime;
	}

	public override BombParams bombParams { 
		get { 
			if (!isRadioactive)
				return _playerController.bombParams;

			return temp;
		} 
		set { 
			temp.explodingDuration = value.explodingDuration;
			temp.warningTime = value.warningTime;
			temp.delayTime = value.delayTime;
			_playerController.bombParams = value; } 
	}
}
