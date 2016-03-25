using UnityEngine.UI;
using UnityEngine;

public class PlayerScoreInfo : MonoBehaviour
{
    public Text username;
    public Text score;

    public void PopulateScoreInfo(string name, int scr)
    {
        username.text = name;
        score.text = scr.ToString();
    }
}
