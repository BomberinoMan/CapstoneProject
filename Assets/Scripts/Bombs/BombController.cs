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
	private bool _isMoving = false;
    private float _startTime;
    private Vector2 _direction = new Vector2();

    void Start()
    {
        if(paramaters == null)
		    paramaters = new BombParams();
    }

	void OnDestroy(){
		if (!_hasExploded && parentPlayer != null) {
			parentPlayer.GetComponent<PlayerControllerComponent> ().currNumBombs++;
		}
	}

    void FixedUpdate()
    {
		if (_startTime + paramaters.delayTime + paramaters.warningTime <= Time.time) {
			Explode ();
		}

		if (!isServer)
			return;

		if(_isMoving)
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
				if(parentPlayer != null)
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
		if (collisionInfo.gameObject.tag == "Player")
        {
			//Do all of the bomb kick things on the server
			if (!isServer)
				return;
			
			if (_isMoving)
				StopMovement();
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

	public void Kick(Vector2 direction)
	{
		if (!isServer)
			return;
		_isMoving = true;
		_direction = direction;
	}

    private void StopMovement()
    {
		_isMoving = false;
		_direction = Vector2.zero;
        transform.position = new Vector3(AxisRounder.Round(transform.position.x), AxisRounder.Round(transform.position.y), 0.0f);
    }
}
