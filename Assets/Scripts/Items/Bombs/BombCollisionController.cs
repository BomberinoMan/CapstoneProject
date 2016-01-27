using UnityEngine;
using System.Collections;

public class BombCollisionController : MonoBehaviour {
	public GameObject parentPlayer;

	void OnTriggerExit2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.Equals (parentPlayer))
			Physics2D.IgnoreCollision (gameObject.GetComponent<Collider2D> (), collisionInfo.gameObject.GetComponent<Collider2D> (), false);
	}
}
