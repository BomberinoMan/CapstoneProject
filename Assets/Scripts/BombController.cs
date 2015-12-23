using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour 
{	
	public BombParams paramaters;
    public GameObject parentPlayer;

	void Start()
	{
		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.AddBomb (gameObject, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y, paramaters);
	}

	public void Explode()
	{
		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.ExplodeBomb ((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
	}
}
