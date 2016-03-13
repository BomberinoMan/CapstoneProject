using UnityEngine;
using UnityEngine.Networking;

public class BombController : NetworkBehaviour
{
    public float speed;
    public float speedScalar;
    public BombParams paramaters;
    public GameObject parentPlayer;

    private bool _hasExploded = false;
    private bool _isMoving = false;
    private float _startTime;
    private Vector3 _direction = new Vector3();
    private Rigidbody2D _rb;

    void Start()
    {
        //TODO need to handle the case where two players are on top of the bomb on instantiation
        _rb = GetComponent<Rigidbody2D>();
        // Set parentPlayer in all child sub colliders
        foreach (BombCollisionController collisionController in gameObject.GetComponentsInChildren<BombCollisionController>())
            collisionController.parentPlayer = parentPlayer;

        parentPlayer.GetComponent<PlayerControllerComponent>().currNumBombs--;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

	void OnDestroy(){
		if (!_hasExploded) {
			parentPlayer.GetComponent<PlayerControllerComponent> ().currNumBombs++;
		}
	}

    void FixedUpdate()
    {
		if (_startTime + paramaters.delayTime <= Time.time) {
			Explode ();
		}

        if (_isMoving)
        {
            _rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            if ((int)_direction.x != 0)
            {
                _rb.constraints = _rb.constraints | RigidbodyConstraints2D.FreezePositionY;
                _rb.position = new Vector3(
                    _rb.position.x + _direction.x * speed * speedScalar,
                    AxisRounder.Round(_rb.position.y),
                    0.0f);
            }
            else if ((int)_direction.y != 0)
            {
                _rb.constraints = _rb.constraints | RigidbodyConstraints2D.FreezePositionX;
                _rb.position = new Vector3(
                    AxisRounder.Round(_rb.position.x),
                    _rb.position.y + _direction.y * speed * speedScalar,
                    0.0f);
            }
        }
    }

    public void SetupBomb(GameObject player, bool setupColliders = true)
    {
        _startTime = Time.time;
        paramaters = new BombParams();
        BombParams playerBombParams = player.GetComponent<PlayerControllerComponent>().bombParams;

        paramaters.delayTime = playerBombParams.delayTime;
        paramaters.explodingDuration = playerBombParams.explodingDuration;
        paramaters.radius = playerBombParams.radius;
        paramaters.warningTime = playerBombParams.warningTime;

        parentPlayer = player;
        transform.SetParent(player.transform.parent);

        if (setupColliders)
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())   // Ignore collisions on all child colliders aswell, do not ignore colliders that are triggers
                if (!collider.isTrigger)
                    Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>());
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
                gameObject.GetComponent<LaserInstantiator>().InstantiateLaser();
				Destroy(gameObject);
            }
        }
        catch (MissingReferenceException)
        {
        }
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
		if (collisionInfo.gameObject.tag == "Player" && collisionInfo.gameObject.GetComponent<PlayerControllerComponent>().bombKick > 0)
        {
            foreach (Collider2D bombCollider in gameObject.GetComponentsInChildren<Collider2D>())
                if (Physics2D.GetIgnoreCollision(bombCollider, collisionInfo.gameObject.GetComponent<Collider2D>()))
                    // Ignore collisions on colliders that are on the parent player before they leave the bomb
                    return;

            _isMoving = true;
            _direction = -(collisionInfo.transform.position - transform.position).normalized;
            _direction.x = AxisRounder.Round(_direction.x);
            _direction.y = AxisRounder.Round(_direction.y);
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
        _isMoving = false;
        _rb.position = new Vector3(AxisRounder.Round(_rb.position.x), AxisRounder.Round(_rb.position.y), 0.0f);
    }
}
