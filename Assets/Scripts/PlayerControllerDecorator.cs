using UnityEngine;

public class PlayerControllerDecorator : MonoBehaviour, IPlayerController {
	public float startTime;
	public float duration;

	private IPlayerController _playerController;

	public PlayerControllerDecorator(IPlayerController playerController) {
		_playerController = playerController;
	}

	void Start () {
		startTime = Time.time;
		duration = 10.0f; //TODO This needs to be implemented better, value also probaly needs to be changed
	}

	void Update () {
		if (startTime + duration <= Time.time)
			removeRadioactive ();
	}

	public void removeRadioactive() {
		// TODO
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

	public BombParams bombParams { 
		get { return _playerController.bombParams; }
		set { _playerController.bombParams = value; } 
	}

	public bool canLayBombs { 
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
