using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed;

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertizal = Input.GetAxis ("Vertical");

		GetComponent<Rigidbody> ().velocity = new Vector3 (moveHorizontal, 0.0f, moveVertizal) * speed;
		GetComponent<Rigidbody> ().position = new Vector3
			(
				Mathf.Clamp(GetComponent<Rigidbody> ().position.x, xMin, xMax), 
				0.0f, 
		    	Mathf.Clamp(GetComponent<Rigidbody> ().position.z, zMin, zMax)
			);
	}
}
