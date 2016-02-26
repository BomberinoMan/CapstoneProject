using UnityEngine.UI;
using UnityEngine;

public class ScoreScreenPlayerScore : MonoBehaviour {
    public void SetPlayerScore(int score)
    {
        GetComponent<Text>().text = score.ToString();
    }
}
