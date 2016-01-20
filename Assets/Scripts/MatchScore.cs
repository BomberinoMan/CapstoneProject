using UnityEngine;
using System;

public class MatchScore {
	public int[] playerScores;

	public MatchScore () {
		playerScores = new int[4];

		playerScores [0] = -1;
		playerScores [1] = -1;
		playerScores [2] = -1;
		playerScores [3] = -1;
	}

	public void IncrementScore(int playerNum){
		if (playerNum < 0 || playerNum >= 4)
			throw new ArgumentOutOfRangeException ();


		playerScores [playerNum]++;
	}
}
