using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyPlayerInfo : MonoBehaviour
{
    public Text username;
    public Image color;
    public Text readyStatus;
    public Button readyButton;

    public void PopulatePlayerInfo(string usr, int position, bool isReady)
    {

        username.text = usr;

        switch (position)
        {
            case 0:
                color.color = Color.blue;
                break;
            case 1:
                color.color = Color.yellow;
                break;
            case 2:
                color.color = Color.red;
                break;
            case 3:
                color.color = Color.green;
                break;
            default:
                color.color = Color.white;
                break;
        }

        if (isReady)
        {
            readyStatus.text = "READY";
        }
        else
        {
            readyStatus.text = "NOT READY";
        }
    }
}
