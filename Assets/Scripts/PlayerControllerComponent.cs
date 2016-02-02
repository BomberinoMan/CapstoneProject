using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class PlayerControllerComponent : NetworkBehaviour
{
    [SyncVar]
    public int playerIndex;

    private LobbyManager _lobbyManager;
    private float _speed;
    private float _flipFlopTime;
    private float _flipFlopDuration = 0.25f;
    private IPlayerControllerModifier _playerController;
    private bool _bombBlock = false;

    public GameObject bombObject;
    private Rigidbody2D _rb;
    private Transform _transform;

    public int currNumBombs { get { return _playerController.currNumBombs; } set { _playerController.currNumBombs = value; } }
    public int maxNumBombs { get { return _playerController.maxNumBombs; } set { _playerController.maxNumBombs = value; } }
    public int bombKick { get { return _playerController.bombKick; } set { _playerController.bombKick = value; } }
    public int bombLine { get { return _playerController.bombLine; } set { _playerController.bombLine = value; } }
    public BombParams bombParams { get { return _playerController.bombParams; } set { _playerController.bombParams = value; } }

    public override void OnStartClient()
    {
        if (_lobbyManager == null)
            _lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();

        GameObject animator = Instantiate(_lobbyManager.playerAnimations[playerIndex]) as GameObject;
        animator.transform.SetParent(gameObject.transform);
        // Changing the parent also changes the localPosition, need to reset it
        animator.transform.localPosition = Vector3.zero;

        // To sync animations, we need to assign the NetworkAnimator to the new child animator
        //          The animator on this (parent) object will be disabled
        GetComponent<NetworkAnimator>().animator = GetComponentsInChildren<Animator>().Where(x => x.enabled).First();
    }

    public void Start()
    {
        if (_lobbyManager == null)
            _lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        _speed = 0.06f;
        _flipFlopTime = Time.time;
        _playerController = new DefaultPlayerControllerModifier();
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown("space") && _playerController.canLayBombs)
        {
            if (!_bombBlock)
            {
                CmdLayBomb();
            }
            else if (_playerController.bombLine > 0)
            {
				CmdLayLineBomb(gameObject.GetComponentInChildren<PlayerAnimationDriver>().GetDirection());
            }
        }
        if (_playerController.alwaysLayBombs && !_bombBlock)
        {
            CmdLayBomb();
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

    [ClientRpc]
	private void RpcSetupBomb(GameObject player, GameObject bomb, bool setupColliders)
    {
		BombManager.SetupBomb(gameObject, bomb, setupColliders);
    }

    [Command]
    private void CmdLayBomb()
    {
        if (_playerController.currNumBombs <= 0)
        {
            return;
        }


        GameObject bomb = Instantiate(
            bombObject,
            new Vector3(
                AxisRounder.Round(_transform.position.x),
                AxisRounder.Round(_transform.position.y),
                0.0f),
            Quaternion.identity)
            as GameObject;

        BombManager.SetupBomb(gameObject, bomb);
        NetworkServer.SpawnWithClientAuthority(bomb, gameObject);
		RpcSetupBomb(gameObject, bomb, true);
    }

    [Command]
	private void CmdLayLineBomb(Vector2 direction)
    {
		var emptySpace = Physics2D.RaycastAll(new Vector2(AxisRounder.Round(_transform.position.x), AxisRounder.Round(transform.position.y)), direction)
			.Where(x => x.distance != 0)
			.First();

			//The number of bombs that will be spawned is either the amount of clear space, or the number of bombs
		int numBombs = emptySpace.distance < currNumBombs ? (int)emptySpace.distance : currNumBombs;

		for (int i = 1; i <= numBombs; i++) {
			GameObject bomb = Instantiate(
				bombObject,
				new Vector3(
					AxisRounder.Round(_transform.position.x + direction.x * i),
					AxisRounder.Round(_transform.position.y + direction.y * i),
					0.0f),
				Quaternion.identity)
				as GameObject;

			BombManager.SetupBomb(gameObject, bomb, false);
			NetworkServer.SpawnWithClientAuthority(bomb, gameObject);
			RpcSetupBomb(gameObject, bomb, false);
		}
    }

	[Command]
	private void CmdPickedUpUpgrade(GameObject upgrade){
		NetworkServer.Destroy (upgrade);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Upgrade")
        {
            UpgradeFactory.getUpgrade(other.gameObject.GetComponent<UpgradeTypeComponent>().type).ApplyEffect(gameObject);
			CmdPickedUpUpgrade(other.gameObject);
        }
        else if (other.gameObject.tag == "Laser")
        {
            //TODO add destruction animation support
			CmdPickedUpUpgrade(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "BombBlock")
            _bombBlock = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "BombBlock")
            _bombBlock = false;
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
