using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreScreenPlayerScore : NetworkBehaviour {
    public void SetPlayerScore(int score)
    {
        GetComponent<Text>().text = score.ToString();
    }
}
