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

	void Start()
	{
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
}
