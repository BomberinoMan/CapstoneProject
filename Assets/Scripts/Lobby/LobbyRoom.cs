using UnityEngine;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour
{
    public static LobbyRoom instance = null;
    public RectTransform playerListContentTransform;
	public Text roomTitle;
    public Button leaveButton;

    public void OnEnable()
    {
        instance = this;
    }
		
    public void AddPlayer(LobbyPlayer player)
    {
        player.transform.SetParent(playerListContentTransform, false);
    }

	public void SetRoomName(string roomName){
		roomTitle.text = roomName;
	}
}
