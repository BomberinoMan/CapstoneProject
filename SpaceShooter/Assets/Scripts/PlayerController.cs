using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public float fireRatePerSecond;
	private float nextFire;

	public Boundary boundary;
	public GameObject bolt;
	public Transform boltSpawn;

	void Update()
	{
		if(Input.GetButton("Fire1") && Time.time > nextFire)
		{
			nextFire = Time.time + fireRatePerSecond;
			Instantiate(bolt, boltSpawn.position, boltSpawn.rotation);
			GetComponent<AudioSource> ().Play();
		}
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertizal = Input.GetAxis ("Vertical");

		GetComponent<Rigidbody> ().velocity = new Vector3 (moveHorizontal, 0.0f, moveVertizal) * speed;
		GetComponent<Rigidbody> ().position = new Vector3
			(
				Mathf.Clamp(GetComponent<Rigidbody> ().position.x, boundary.xMin, boundary.xMax), 
				0.0f, 
		    	Mathf.Clamp(GetComponent<Rigidbody> ().position.z, boundary.zMin, boundary.zMax)
			);
	}
}
