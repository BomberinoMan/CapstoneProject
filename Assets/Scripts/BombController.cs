using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour 
{	
	public float speed;
	public float speedScalar;
	public BombParams paramaters;
    public GameObject parentPlayer;
	private bool hasExploded = false;

	private bool isMoving = false;
	private string direction = "";
	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();

			// Ignor collisions with player that planted the bomb
		if (GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ().OnPlayer ((int)gameObject.transform.position.x, (int)gameObject.transform.position.y)) {
			Physics2D.IgnoreCollision (gameObject.GetComponent<Collider2D> (), parentPlayer.gameObject.GetComponent<Collider2D> ());
			foreach(Collider2D collider in gameObject.GetComponentsInChildren<Collider2D> ())	// Ignore collisions on all child colliders aswell, do not ignore colliders that are triggers
				if(collider.gameObject.GetInstanceID() != GetInstanceID() && !collider.isTrigger)
					Physics2D.IgnoreCollision (collider, parentPlayer.gameObject.GetComponent<Collider2D> ());
			gameObject.GetComponentInChildren<BombCollisionController> ().parentPlayer = parentPlayer;
		}

		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.AddBomb (gameObject);

		parentPlayer.GetComponent<PlayerController>().currNumBombs--;
		GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
	}

	void Update()
	{
		if (isMoving) {
			rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
			switch(direction)
			{
				case ("Up"):
					rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionX;
					rb.position = new Vector3(
						AxisRounder.Round(rb.position.x),
						rb.position.y + 1 * speed * speedScalar,
						0.0f);
					break;
				case ("Down"):
					rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionX;
					rb.position = new Vector3(
						AxisRounder.Round(rb.position.x),
						rb.position.y - 1 * speed * speedScalar,
						0.0f);
					break;
				case ("Left"):
					rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionY;
					rb.position = new Vector3(
						rb.position.x - 1 * speed * speedScalar,
						AxisRounder.Round(rb.position.y),
						0.0f);
					break;
				case ("Right"):
					rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionY;
					rb.position = new Vector3(
						rb.position.x + 1 * speed * speedScalar,
						AxisRounder.Round(rb.position.y),
						0.0f);
					break;
				default:
					break;
			}
		}

	}

	public void Explode(bool notifyBoard = true)
	{
		if(notifyBoard)
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
				.ExplodeBomb ((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);

		try
		{
			if(!hasExploded)
				parentPlayer.GetComponent<PlayerController>().currNumBombs++;
			hasExploded = true;
		}

#pragma warning disable CS0168 // Variable is declared but never used
        catch (MissingReferenceException e) // If the player dies before the bomb explodes, then we do not need to give them another one
#pragma warning restore CS0168 // Variable is declared but never used
        { }
	}
	
	void OnTriggerExit2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.Equals (parentPlayer))
			Physics2D.IgnoreCollision (gameObject.GetComponent<Collider2D> (), collisionInfo.gameObject.GetComponent<Collider2D> (), false);
	}

	void OnTriggerEnter2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player" && collisionInfo.gameObject.GetComponent<PlayerController> ().bombKick > 0) {
			if (collisionInfo.gameObject.Equals (parentPlayer) && Physics2D.GetIgnoreCollision (gameObject.GetComponent<Collider2D> (), collisionInfo.gameObject.GetComponent<Collider2D> ()))
				return;
			isMoving = true;
			direction = collisionInfo.gameObject.GetComponentInChildren<PlayerAnimationDriver> ().GetDirection ();
		} 
		else if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb") {
			StopMovement();
		}
        else if(collisionInfo.gameObject.tag == "Upgrade")
        {
            Destroy(collisionInfo.gameObject);
        }
	}

	void OnTriggerStay2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb") {
			StopMovement();
		}
	}

	private void StopMovement()
	{
		isMoving = false;
		direction = "";

		rb.position = new Vector3 (AxisRounder.Round (rb.position.x), AxisRounder.Round (rb.position.y), 0.0f);
	}
}
