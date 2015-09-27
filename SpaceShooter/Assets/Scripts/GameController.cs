using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	public GameObject[] asteroids;
	public Vector3 spawnRange;

	public float waveWait;
	public float spawnWait;
	public float spawnWaitDecrement;
	public float spawnWaitMin;
	public float waveSize;
	public float waveSizeIncrement;

	public GUIText scoreText;
	//public float scoreMultiplier;
	private int score;

	void Start()
	{
		score = 0;
		//scoreMultiplier = 1.0f;
		UpdateScore ();
		StartCoroutine (SpawnRandomAsteroid());
	}

	IEnumerator SpawnRandomAsteroid()
	{
		while (true) 
		{
			yield return new WaitForSeconds(waveWait);

			for(int i = 0; i < waveSize; i++)
			{
				Vector3 spawnPosition = new Vector3 (Random.Range(-spawnRange.x, spawnRange.x), spawnRange.y, spawnRange.z);
				Quaternion spawnRotation = Quaternion.identity;
	            Instantiate (asteroids[(int)Random.Range(0.0f, asteroids.Length)], spawnPosition, spawnRotation);

				yield return new WaitForSeconds(spawnWait);
			}

			waveSize += waveSizeIncrement;

			if(spawnWait > spawnWaitMin)
				spawnWait -= spawnWaitDecrement;
		}
	}

	public void UpdateScore(int newIncrementScore)
	{
		score += (int)(newIncrementScore);// * scoreMultiplier);
		UpdateScore();
	}

	void UpdateScore()
	{
		scoreText.text = "Score: " + score;
	}
}
