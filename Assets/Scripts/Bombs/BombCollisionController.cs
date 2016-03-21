using UnityEngine;
using System.Linq;

public class BombCollisionController : MonoBehaviour
{
	void Start(){
		var horizontal = gameObject.transform.position;
		horizontal.x -= 0.45f;
		var vertical = gameObject.transform.position;
		vertical.y -= 0.45f;

		var players = Physics2D.RaycastAll (horizontal, Vector2.right, 0.9f).Where (x => x.transform.tag == "Player").ToList ();
		foreach (var player in players)
			Physics2D.IgnoreCollision (player.collider, GetComponent<Collider2D> ());

		players = Physics2D.RaycastAll (vertical, Vector2.up, 0.9f).Where (x => x.transform.tag == "Player").ToList ();
		foreach (var player in players)
			Physics2D.IgnoreCollision (player.collider, GetComponent<Collider2D> ());
	}

    void OnTriggerExit2D(Collider2D collisionInfo)
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collisionInfo.gameObject.GetComponent<Collider2D>(), false);
    }
}
