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
	private Vector3 direction = new Vector3();
	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();

			// Ignor collisions with player that planted the bomb
		if (GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ().OnPlayer ((int)AxisRounder.Round(gameObject.transform.position.x), (int)AxisRounder.Round(gameObject.transform.position.y))) {
			foreach (Collider2D collider in gameObject.GetComponentsInChildren<Collider2D> ())	// Ignore collisions on all child colliders aswell, do not ignore colliders that are triggers
				if (!collider.isTrigger)
					Physics2D.IgnoreCollision (collider, parentPlayer.gameObject.GetComponent<Collider2D> ());
				
				// set parentPlayer in all child sub colliders
			foreach (BombCollisionController collisionController in gameObject.GetComponentsInChildren<BombCollisionController> ())
				collisionController.parentPlayer = parentPlayer;
		}

		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.AddBomb (gameObject);

		parentPlayer.GetComponent<PlayerControllerComponent>().currNumBombs--;
		GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
	}

	void FixedUpdate()
	{
		if (isMoving) {
			rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
			if ((int)direction.x != 0) {
				rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionY;
				rb.position = new Vector3 (
					rb.position.x + direction.x * speed * speedScalar,
					AxisRounder.Round (rb.position.y),
					0.0f);
			} else if ((int)direction.y != 0) {
				rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionX;
				rb.position = new Vector3 (
					AxisRounder.Round (rb.position.x),
					rb.position.y + direction.y * speed * speedScalar,
					0.0f);
			}
		}
	}

	public void Explode(bool notifyBoard = true)
	{
		StopMovement ();
		if(notifyBoard)
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
				.ExplodeBomb ((int)AxisRounder.Round(gameObject.transform.position.x), (int)AxisRounder.Round(gameObject.transform.position.y));

		try {
			if(!hasExploded)
				parentPlayer.GetComponent<PlayerControllerComponent>().currNumBombs++;
			hasExploded = true;
		}

		#pragma warning disable CS0168 // Variable is declared but never used
		catch (MissingReferenceException e) { } // If the player dies before the bomb explodes, then we do not need to give them another one
		#pragma warning restore CS0168 // Variable is declared but never used
	}

    //TODO Add collision detection for lasers
	void OnTriggerEnter2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player" && collisionInfo.gameObject.GetComponent<PlayerControllerComponent> ().bombKick > 0) {
			if (collisionInfo.gameObject.Equals (parentPlayer) && Physics2D.GetIgnoreCollision (gameObject.GetComponent<Collider2D> (), collisionInfo.gameObject.GetComponent<Collider2D> ()))
				return;
			
			isMoving = true;
			direction = -(collisionInfo.transform.position - transform.position).normalized;
			direction.x = AxisRounder.Round (direction.x);
			direction.y = AxisRounder.Round (direction.y);
		} else if (collisionInfo.gameObject.tag == "Blocking" || collisionInfo.gameObject.tag == "Bomb") {
			StopMovement ();
		} else if (collisionInfo.gameObject.tag == "Upgrade") {
			Destroy (collisionInfo.gameObject);
		} else if (collisionInfo.gameObject.tag == "Laser") {
			Explode ();
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
		direction = Vector3.zero;

		rb.position = new Vector3 (AxisRounder.Round (rb.position.x), AxisRounder.Round (rb.position.y), 0.0f);
	}
}
