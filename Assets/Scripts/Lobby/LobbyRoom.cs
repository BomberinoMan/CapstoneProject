using UnityEngine;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour
{
    public static LobbyRoom instance = null;
    public RectTransform playerListContentTransform;
    public Button leaveButton;

    public void OnEnable()
    {
        instance = this;
    }
		
    public void AddPlayer(LobbyPlayer player)
    {
        player.transform.SetParent(playerListContentTransform, false);
    }
}
