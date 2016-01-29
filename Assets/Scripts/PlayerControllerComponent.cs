using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControllerComponent : NetworkBehaviour
{
	[SyncVar]
	public int playerNum;

	private LobbyManager _lobbyManager;
    private float _speed;
    private float _flipFlopTime;
    private float _flipFlopDuration = 0.25f;
    private IPlayerControllerModifier _playerController;

    public GameObject bombObject;
    private Rigidbody2D _rb;
    private Transform _tempTransform;

    public int currNumBombs { get { return _playerController.currNumBombs; } set { _playerController.currNumBombs = value; } }
    public int maxNumBombs { get { return _playerController.maxNumBombs; } set { _playerController.maxNumBombs = value; } }
    public int bombKick { get { return _playerController.bombKick; } set { _playerController.bombKick = value; } }
    public int bombLine { get { return _playerController.bombLine; } set { _playerController.bombLine = value; } }
    public BombParams bombParams { get { return _playerController.bombParams; } set { _playerController.bombParams = value; } }

	public override void OnStartClient(){
		if (isServer)
			return;
		
		if(_lobbyManager == null)
			_lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager>();

		GameObject animator = Instantiate(_lobbyManager.playerAnimations [playerNum]) as GameObject;
		animator.transform.SetParent (gameObject.transform);
			// Changing the parent also changes the localPosition, need to reset it
		animator.transform.localPosition = Vector3.zero; 	
	}

    void Start()
    {
		if(_lobbyManager == null)
			_lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager>();
        _speed = 0.06f;
        _flipFlopTime = Time.time;
        _playerController = new DefaultPlayerControllerModifier();
        _rb = GetComponent<Rigidbody2D>();
        _tempTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space") && _playerController.canLayBombs)
        {
            if (!OnBomb())
            {
                LayBomb();
            }
            else if (_playerController.bombLine > 0)
            {
                LayLineBomb();
            }
        }

        if (_playerController.alwaysLayBombs && !OnBomb())
        {
            LayBomb();
        }
    }

    void FixedUpdate() //TODO Add reverse movement support to animation driver
    {
		if (!isLocalPlayer)
			return;
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (!_playerController.reverseMovement)
        {
            if (ver == 0.0f && hor != 0.0f)
            {
                _rb.position = new Vector3(
                    _rb.position.x + hor * _speed * _playerController.speedScalar,
                    AxisRounder.SmoothRound(0.3f, 0.7f, _rb.position.y),
                    0.0f);
            }
            else if (hor == 0.0f && ver != 0.0f)
            {
                _rb.position = new Vector3(
                    AxisRounder.SmoothRound(0.3f, 0.7f, _rb.position.x),
                    _rb.position.y + ver * _speed * _playerController.speedScalar,
                    0.0f);
            }
        }
        else
        {
            if (ver == 0.0f && hor != 0.0f)
            {
                _rb.position = new Vector3(
                    _rb.position.x - hor * _speed * _playerController.speedScalar,
                    AxisRounder.SmoothRound(0.3f, 0.7f, _rb.position.y),
                    0.0f);
            }
            else if (hor == 0.0f && ver != 0.0f)
            {
                _rb.position = new Vector3(
                    AxisRounder.SmoothRound(0.3f, 0.7f, _rb.position.x),
                    _rb.position.y - ver * _speed * _playerController.speedScalar,
                    0.0f);
            }
        }

        //flipFlopColor(); //TODO uncomment this when ready
    }

    private void LayBomb()
    {
        if (_playerController.currNumBombs <= 0)
        {
            return;
        }

        GameObject bomb = Instantiate(
            bombObject,
            new Vector3(
                AxisRounder.Round(_tempTransform.position.x),
                AxisRounder.Round(_tempTransform.position.y),
                0.0f),
            Quaternion.identity)
            as GameObject;
        BombManager.SetupBomb(gameObject, bomb);
    }

    private void LayLineBomb()
    {
        GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<BoardManager>()
            .LineBomb(
                (int)AxisRounder.Round(_tempTransform.position.x),
                (int)AxisRounder.Round(_tempTransform.position.y),
                gameObject.GetComponentInChildren<PlayerAnimationDriver>().GetDirection(),
                _playerController.currNumBombs,
                gameObject);
    }

    private bool OnBomb()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().OnBomb((int)AxisRounder.Round(_tempTransform.position.x), (int)AxisRounder.Round(_tempTransform.position.y));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Upgrade")
        {
            UpgradeFactory.getUpgrade(other.gameObject.GetComponent<UpgradeType>().type).ApplyEffect(gameObject);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Laser")
        {
            //TODO add destruction animation support
            //Destroy(gameObject);
        }
    }

    public IPlayerControllerModifier getPlayerControllerModifier()
    {
        return _playerController;
    }

    public void changePlayerControllerModifier(IPlayerControllerModifier newModifier)
    {
        _playerController = newModifier;
    }

    private void flipFlopColor()
    {
        if (!_playerController.isRadioactive)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            return;
        }

        if (_flipFlopTime + _flipFlopDuration > Time.time)
        {
            return;
        }

        if (gameObject.GetComponentInChildren<SpriteRenderer>().color == Color.white)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(0.678f, 0.698f, 0.741f);
        }
        else
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }

        _flipFlopTime = Time.time;
    }
}
