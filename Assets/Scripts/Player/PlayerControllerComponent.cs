using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayerControllerComponent : NetworkBehaviour
{
    [SyncVar]
    public int slot;

    private float _speed;
    private float _flipFlopTime;
    private float _flipFlopDuration = 0.25f;
    private IPlayerControllerModifier _playerController;
    private bool _bombBlock = false;
    private bool _animatorSetup = false;

	public DPadController dPad;
    public GameObject bombObject;
    private Rigidbody2D _rb;
    private Transform _transform;

    public int currNumBombs { get { return _playerController.currNumBombs; } set { _playerController.currNumBombs = value; } }
    public int maxNumBombs { get { return _playerController.maxNumBombs; } set { _playerController.maxNumBombs = value; } }
    public int bombKick { get { return _playerController.bombKick; } set { _playerController.bombKick = value; } }
    public int bombLine { get { return _playerController.bombLine; } set { _playerController.bombLine = value; } }
    public BombParams bombParams { get { return _playerController.bombParams; } set { _playerController.bombParams = value; } }
    public bool reverseMovement { get { return _playerController.reverseMovement; } }

    public override void OnStartClient()
    {
        base.OnStartClient();

		GameObject animator = Instantiate(GameManager.instance.playerAnimations[slot]) as GameObject;
        animator.transform.SetParent(gameObject.transform);
        // Changing the parent also changes the localPosition, need to reset it
        animator.transform.localPosition = Vector3.zero;

        // To sync animations, we need to assign the NetworkAnimator to the new child animator
        //          The animator on this (parent) object will be disabled
        GetComponent<NetworkAnimator>().animator = GetComponentsInChildren<Animator>().Where(x => x.enabled).First();
        _animatorSetup = true;
    }

	public override void OnStartLocalPlayer(){
		// Get the touch input from the UI
		dPad = GameObject.Find("DPadArea").GetComponent<DPadController>();
		GameObject.Find ("BombArea").GetComponent<TouchBomb> ().SetPlayerController (this);
	}

    public void Start()
    {
        _speed = 0.06f;
        _flipFlopTime = Time.time;
        _playerController = new DefaultPlayerControllerModifier();
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
    }

	public void TouchLayBomb(bool fromTouch = true)
    {
		if (!isLocalPlayer)
            return;

        if (_playerController.canLayBombs && _playerController.currNumBombs > 0)
        {
            if (!_bombBlock)
            {
                CmdLayBomb(bombParams.delayTime, bombParams.explodingDuration, bombParams.radius, bombParams.warningTime);
            }
            else if (_playerController.bombLine > 0 && fromTouch)
            {
                CmdLayLineBomb(bombParams.delayTime, bombParams.explodingDuration, bombParams.radius, bombParams.warningTime, gameObject.GetComponentInChildren<PlayerAnimationDriver>().GetDirection());
            }
        }
    }

    void FixedUpdate() //TODO Add reverse movement support to animation driver
    {
		FlipFlopColor();

        if (_playerController.alwaysLayBombs)
            TouchLayBomb(false);

        if (!isLocalPlayer)
            return;

        float hor = dPad.currDirection.x;
        float ver = dPad.currDirection.y;

		if (hor == 0 && ver == 0) {
			hor = Input.GetAxisRaw ("Horizontal");
			ver = Input.GetAxisRaw ("Vertical");
		}

        if (_playerController.reverseMovement)
        {
            hor *= -1.0f;
            ver *= -1.0f;
        }

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

    [ClientRpc]
    private void RpcSetupBomb(GameObject bomb, bool setupColliders)
    {
		bomb.GetComponent<BombController>().SetupBomb(gameObject, setupColliders);
    }

    [Command]
    private void CmdLayBomb(float delayTime, float explodingDuration, int radius, float warningTime)
    {
        bombParams.delayTime = delayTime;
        bombParams.explodingDuration = explodingDuration;
        bombParams.radius = radius;
        bombParams.warningTime = warningTime;

        GameObject bomb = Instantiate(
            bombObject,
            new Vector3(
                AxisRounder.Round(_transform.position.x),
                AxisRounder.Round(_transform.position.y),
                0.0f),
            Quaternion.identity)
            as GameObject;
		
		bomb.GetComponent<BombController>().SetupBomb(gameObject);
		NetworkServer.SpawnWithClientAuthority (bomb, gameObject);
		RpcSetupBomb(bomb, true);
    }

    [Command]
    private void CmdLayLineBomb(float delayTime, float explodingDuration, int radius, float warningTime, Vector2 direction)
    {
        bombParams.delayTime = delayTime;
        bombParams.explodingDuration = explodingDuration;
        bombParams.radius = radius;
        bombParams.warningTime = warningTime;

        var emptySpace = Physics2D.RaycastAll(new Vector2(AxisRounder.Round(_transform.position.x), AxisRounder.Round(transform.position.y)), direction)
            .Where(x => x.distance != 0)
            .First();

        //The number of bombs that will be spawned is either the amount of clear space, or the number of bombs
        int numBombs = emptySpace.distance < currNumBombs ? (int)emptySpace.distance : currNumBombs;

        for (int i = 1; i <= numBombs; i++)
        {
            GameObject bomb = Instantiate(
                bombObject,
                new Vector3(
                    AxisRounder.Round(_transform.position.x + direction.x * i),
                    AxisRounder.Round(_transform.position.y + direction.y * i),
                    0.0f),
                Quaternion.identity)
                as GameObject;

			bomb.GetComponent<BombController>().SetupBomb(gameObject, false);
			NetworkServer.SpawnWithClientAuthority(bomb, gameObject);
			RpcSetupBomb(bomb, false);
        }
    }

    [Command]
    private void CmdDestroyGameObject(GameObject upgrade)
    {
        NetworkServer.Destroy(upgrade);
    }

    [Command]
    public void CmdPlayerHit(uint bombNetId)
    {
        GameManager.instance.PlayerHit(this, bombNetId, true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.tag == "Upgrade") {
			//TODO sync effects of radioactive upgrades. Need to go to server to get the type, then apply the effect with RPC call
            //TODO, possibly make another class that doesn't actually apply radioactive effects but just makes the player turn colors
			UpgradeFactory.GetUpgrade (other.gameObject.GetComponent<UpgradeTypeComponent> ().type).ApplyEffect (gameObject);
			if (localPlayerAuthority)
				CmdDestroyGameObject (other.gameObject);
		} else if (other.gameObject.tag == "Laser") {
            if (isServer)
                GameManager.instance.PlayerHit(this, other.gameObject.GetComponent<LaserController>().bombNetId);
            else
                CmdPlayerHit(other.gameObject.GetComponent<LaserController>().bombNetId);
		} else if (other.gameObject.tag == "BombBlock") {
			_bombBlock = true;
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

    public IPlayerControllerModifier GetPlayerControllerModifier()
    {
        return _playerController;
    }

    public void ChangePlayerControllerModifier(IPlayerControllerModifier newModifier)
    {
        _playerController = newModifier;
    }

    private void FlipFlopColor() // TODO sync color swap
    {
        if (!_animatorSetup)
            return;

        if (!_playerController.isRadioactive)
        {
            gameObject.GetComponentsInChildren<SpriteRenderer>().Where(x => x.enabled).First().color = Color.white;
            return;
        }

        if (_flipFlopTime + _flipFlopDuration > Time.time)
        {
            return;
        }

        if (gameObject.GetComponentsInChildren<SpriteRenderer>().Where(x => x.enabled).First().color == Color.white)
        {
            gameObject.GetComponentsInChildren<SpriteRenderer>().Where(x => x.enabled).First().color = new Color(0.2f, 0.2f, 0.2f);
        }
        else
        {
            gameObject.GetComponentsInChildren<SpriteRenderer>().Where(x => x.enabled).First().color = Color.white;
        }

        _flipFlopTime = Time.time;
    }
}
