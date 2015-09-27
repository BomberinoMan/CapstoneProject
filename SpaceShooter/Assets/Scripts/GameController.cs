using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	public GameObject[] asteroids;
	public Vector3 spawnRange;
    public GUIText scoreText;
    public GUIText gameOverText;
    public GUIText restartText;

    public float waveWait;
	public float spawnWait;
	public float spawnWaitDecrement;
	public float spawnWaitMin;
	public float waveSize;
	public float waveSizeIncrement;
	
	private int score;
    private bool gameOver;

	void Start()
	{
		score = 0;
        gameOverText.text = "";
        restartText.text = "";
		UpdateScore ();
		StartCoroutine (SpawnRandomAsteroid());
	}

    void Update()
    {
        if(gameOver)
        {
            if(Input.GetKeyDown (KeyCode.R))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
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

            if (gameOver)
                break;
		}
	}

	public void UpdateScore(int newIncrementScore)
	{
		score += (int)(newIncrementScore);
		UpdateScore();
	}

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        restartText.text = "Press 'R' to Restart";
        gameOver = true;
    }

	void UpdateScore()
	{
		scoreText.text = "Score: " + score;
	}
}
