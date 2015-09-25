using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public GameObject enemyExplosion;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Boundary") 
		{
			if(other.tag == "Player")
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			if(other.tag == "Enemy")
				Instantiate(enemyExplosion, other.transform.position, other.transform.rotation);

			Instantiate(explosion, transform.position, transform.rotation);
			Destroy (other.gameObject);
			Destroy (gameObject);
		}
	}
}
