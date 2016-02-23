using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreScreenPlayerName : NetworkBehaviour {
    public void SetPlayerName(string playerName)
    {
        GetComponent<Text>().text = playerName;
    }
}
