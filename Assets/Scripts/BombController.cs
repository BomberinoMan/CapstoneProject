using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour 
{	
	public BombParams paramaters;
    public GameObject parentPlayer;

	void Start()
	{
        GetComponent<BombAnimationDriver>().paramaters = paramaters;
		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.AddBomb (gameObject, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y, paramaters);
	}

	public void Explode()
	{
        parentPlayer.GetComponent<PlayerController>().numBombs++;
		GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ()
			.ExplodeBomb ((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
	}
}
