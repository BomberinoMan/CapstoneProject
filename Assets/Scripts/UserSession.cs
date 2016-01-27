using UnityEngine;
using System.Collections;

public class UserSession : MonoBehaviour {
	public int numPlayers;
	public MatchScore matchScore;

	void Awake(){
		//DontDestroyOnLoad (this);
	}

	public UserSession () {
		matchScore = new MatchScore ();
		numPlayers = 0;
	}
}
