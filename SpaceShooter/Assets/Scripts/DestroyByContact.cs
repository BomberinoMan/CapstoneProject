using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public GameObject enemyExplosion;
	private GameController gameController;
	public int scoreValue;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");

		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController>();
		if (gameController == null)
			Debug.LogError ("Cannot find instance of game controller");
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Boundary") 
		{
			if(other.tag == "Player")
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			if(other.tag == "Enemy")
				Instantiate(enemyExplosion, other.transform.position, other.transform.rotation);

			gameController.UpdateScore(scoreValue);
			Instantiate(explosion, transform.position, transform.rotation);
			Destroy (other.gameObject);
			Destroy (gameObject);
		}
	}
}
