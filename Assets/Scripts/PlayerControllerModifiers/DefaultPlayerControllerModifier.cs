using UnityEngine;
using System.Collections;

public class DefaultPlayerControllerModifier : IPlayerControllerModifier {
	public float duration = 5.0f;
	private IPlayerController _playerController;

	public DefaultPlayerControllerModifier(IPlayerController playerController) {
		_playerController = playerController;
	}

	public DefaultPlayerControllerModifier() {
		_playerController = new PlayerController();

		_playerController.speedScalar = 1.0f;
		_playerController.bombKick = 1;
		_playerController.bombLine = 1;
		_playerController.currNumBombs = 10;
		_playerController.maxNumBombs = _playerController.currNumBombs;
		_playerController.bombParams = new BombParams();
		_playerController.bombParams.radius = 8;
		_playerController.canLayBombs = true;
		_playerController.alwaysLayBombs = false;
		_playerController.reverseMovement = false;
		_playerController.isRadioactive = false;
	}

	public virtual IPlayerController removeMod () { 
		return _playerController;
	}

	public virtual float speedScalar {
		get { return _playerController.speedScalar; }
		set { _playerController.speedScalar = value; } 
	}

	public virtual int bombKick {
		get { return _playerController.bombKick; }
		set { _playerController.bombKick = value; } 
	}

	public virtual int bombLine {
		get { return _playerController.bombLine; }
		set { _playerController.bombLine = value; } 
	}

	public virtual int currNumBombs {
		get { return _playerController.currNumBombs; }
		set { _playerController.currNumBombs = value; } 
	}

	public virtual int maxNumBombs {
		get { return _playerController.maxNumBombs; }
		set { _playerController.maxNumBombs = value; } 
	}

	public virtual BombParams bombParams {
		get { return _playerController.bombParams; }
		set { _playerController.bombParams = value; } 
	}

	public virtual bool canLayBombs {
		get { return _playerController.canLayBombs; }
		set { _playerController.canLayBombs = value; } 
	}

	public virtual bool alwaysLayBombs {
		get { return _playerController.alwaysLayBombs; }
		set { _playerController.alwaysLayBombs = value; } 
	}

	public virtual bool reverseMovement {
		get { return _playerController.reverseMovement; }
		set { _playerController.reverseMovement = value; } 
	}

	public virtual bool isRadioactive {
		get { return _playerController.isRadioactive; }
		set { _playerController.isRadioactive = value; } 
	}
}
