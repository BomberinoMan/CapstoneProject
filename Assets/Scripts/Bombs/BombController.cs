using UnityEngine;
using UnityEngine.Networking;

public class BombController : NetworkBehaviour
{
    public float speed;
    public float speedScalar;
    public BombParams paramaters;
    public GameObject parentPlayer;

    private bool _hasExploded = false;
    private float _startTime;
    private Vector2 _direction = new Vector2();
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
		if(paramaters == null)
			paramaters = new BombParams();
    }

	void OnDestroy(){
		if (!_hasExploded) {
			parentPlayer.GetComponent<PlayerControllerComponent> ().currNumBombs++;
		}
	}

    void FixedUpdate()
    {
		if (_startTime + paramaters.delayTime + paramaters.warningTime <= Time.time) {
			Explode ();
		}

		_rb.velocity = _direction;
    }

	public void SetupBomb(GameObject player, float delayTime, float explodingDuration, int radius, float warningTime)
    {
		_startTime = Time.time;

		if (!isServer)
			_startTime -= 0.3f;
		Debug.Log ("SetupBomb");
        paramaters = new BombParams();
        paramaters.delayTime = delayTime;
        paramaters.explodingDuration = explodingDuration;
        paramaters.radius = radius;
        paramaters.warningTime = warningTime;

        parentPlayer = player;
		parentPlayer.GetComponent<PlayerControllerComponent>().currNumBombs--;
        transform.SetParent(player.transform.parent);
    }

    public void Explode()
    {
        StopMovement();

        try
        {
            if (!_hasExploded)
            {
                _hasExploded = true;
                parentPlayer.GetComponent<PlayerControllerComponent>().currNumBombs++;
                gameObject.GetComponent<LaserInstantiator>().InstantiateLaser(GetComponent<NetworkIdentity>().netId.Value);
				Deactivate();
            }
        }
        catch (MissingReferenceException)
        {
        }
    }

    private void DestroyMe()
    {
        NetworkServer.Destroy(gameObject);
    }

	private void Deactivate(){
		gameObject.SetActive (false);
        if (isServer)
            // server will destroy deactivated bombs after 2 seconds, this will give plenty of time for the bombs to explode on all of the clients
            Invoke("DestroyMe", 2);
	}

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
		if (collisionInfo.gameObject.tag == "Player" && collisionInfo.gameObject.GetComponent<PlayerControllerComponent>().bombKick > 0 && collisionInfo.gameObject.GetComponent<PlayerControllerComponent>().isLocalPlayer)
        {
            foreach (Collider2D bombCollider in gameObject.GetComponentsInChildren<Collider2D>())
                if (Physics2D.GetIgnoreCollision(bombCollider, collisionInfo.gameObject.GetComponent<Collider2D>()))
                    // Ignore collisions on colliders that are on the parent player before they leave the bomb
                    return;

            _direction = -(collisionInfo.transform.position - transform.position).normalized;
            _direction.x = AxisRounder.Round(_direction.x);
            _direction.y = AxisRounder.Round(_direction.y);
			_rb.isKinematic = false;
			_direction.x *= (speed / Time.fixedDeltaTime) * speedScalar;
			_direction.y *= (speed / Time.fixedDeltaTime) * speedScalar;
        }
        else if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb")
        {
            StopMovement();
        }
        else if (collisionInfo.gameObject.tag == "Upgrade")
        {
            Destroy(collisionInfo.gameObject);
        }
        else if (collisionInfo.gameObject.tag == "Laser")
        {
            Explode();
        }
    }

    void OnTriggerStay2D(Collider2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb" || collisionInfo.gameObject.tag == "Player")
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        _rb.position = new Vector2(AxisRounder.Round(_rb.position.x), AxisRounder.Round(_rb.position.y));
		_direction = Vector2.zero;
		_rb.isKinematic = true;
    }
}
