using UnityEngine;
using UnityEngine.Networking;

public class BombController : NetworkBehaviour
{
    public float speed;
    public float speedScalar;
    public BombParams paramaters;
    public GameObject parentPlayer;
    public GameObject audioObject;

    private bool _hasExploded = false;
    private float _startTime;
    [SyncVar]
    private Vector2 _direction = new Vector2();

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

        transform.position += new Vector3(_direction.x * speed, _direction.y * speed, 0.0f);
    }

	public void SetupBomb(GameObject player, float delayTime, float explodingDuration, int radius, float warningTime)
    {
		_startTime = Time.time;

		if (!isServer)
			_startTime -= 0.3f;

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
                Instantiate(audioObject);
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

    [Command]
    private void CmdSetDirection(Vector2 newDirection)
    {
        _direction = newDirection;
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
		if (collisionInfo.gameObject.tag == "Player" && collisionInfo.gameObject.GetComponent<PlayerControllerComponent>().hasBombKick && hasAuthority)
        {
            foreach (Collider2D bombCollider in gameObject.GetComponentsInChildren<Collider2D>())
                if (Physics2D.GetIgnoreCollision(bombCollider, collisionInfo.gameObject.GetComponent<Collider2D>()))
                    // Ignore collisions on colliders that are on the parent player before they leave the bomb
                    return;

            if (_direction != Vector2.zero)
                StopMovement();

            var newDirection = new Vector2();
            newDirection = -(collisionInfo.transform.position - transform.position).normalized;
            newDirection.x = AxisRounder.Round(newDirection.x);
            newDirection.y = AxisRounder.Round(newDirection.y);
            CmdSetDirection(newDirection);
        }
		else if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb" || collisionInfo.gameObject.tag == "Destructible")
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

    private void StopMovement()
    {
        transform.position = new Vector3(AxisRounder.Round(transform.position.x), AxisRounder.Round(transform.position.y), 0.0f);
        CmdSetDirection(Vector2.zero);
    }
}
