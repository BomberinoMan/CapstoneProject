using UnityEngine.UI;
using UnityEngine;

public class ScoreScreenPlayerName : MonoBehaviour {
    public void SetPlayerName(string playerName)
    {
        GetComponent<Text>().text = playerName;
    }
}
