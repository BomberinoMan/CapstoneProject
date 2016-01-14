using UnityEngine;
using System.Collections;

public class TinyBombsPlayerControllerModifier : DefaultPlayerControllerModifier {
	private float startTime;
	private IPlayerController _playerController;
	private BombParams temp;

	public TinyBombsPlayerControllerModifier(IPlayerController playerController) {
		startTime = Time.time;
		_playerController = playerController;
		temp = new BombParams ();

		temp.delayTime = _playerController.bombParams.delayTime;
		temp.explodingDuration = _playerController.bombParams.explodingDuration;
		temp.radius = 0;
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
			_playerController.bombParams = value; } 
	}

	public override bool isRadioactive { get { return startTime + duration >= Time.time; } }
}
