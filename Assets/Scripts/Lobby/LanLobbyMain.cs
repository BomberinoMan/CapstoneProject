using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class LanLobbyMain : MonoBehaviour
{
	public Button hostButton;
	public Button backButton;
	public Button refreshButton;
	public RectTransform joinList;
	public GameObject serverInfo;

	public void OnEnable()
	{
		hostButton.onClick.RemoveAllListeners();
		hostButton.onClick.AddListener(OnClickHost);

		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(OnClickBack);

		refreshButton.onClick.RemoveAllListeners ();
		refreshButton.onClick.AddListener (OnClickRefresh);

		OnClickRefresh ();
	}

	public void OnClickHost()
	{
		var response = DBConnection.instance.CreateRooom (new CreateRoomMessage { userId = LoginInformation.guid, name = "Hello World" });
		//TODO show popup and get a room name
		if (response.isSuccessful) {
			LobbyManager.instance.StartHost();
		}
		//TODO popup with error message
	}

	public void OnClickBack()
	{
		Destroy(LobbyManager.instance.gameObject);
		SceneManager.LoadScene("MainMenu");
	}

	public void OnClickRefresh(){
		for(int i = joinList.childCount -1; i >= 0; i--){
			Destroy(joinList.GetChild(i).gameObject);
		}

		var response = DBConnection.instance.ListRooms (new ListRoomsMessage { userId = LoginInformation.guid });

		if(response.isSuccessful)
			foreach (var room in response.rooms) {
				var newServerInfo = Instantiate (serverInfo);

				newServerInfo.GetComponent<LobbyServerInfo>().PopulateMatchInfo(room.name, room.ip);
				newServerInfo.transform.SetParent (joinList.transform, false);
				newServerInfo.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}
		
		if (response.rooms.Length == 0) {
			var newServerInfo = Instantiate (serverInfo);

			newServerInfo.GetComponent<LobbyServerInfo>().PopulateMatchInfo("No games exist", "", false);
			newServerInfo.transform.SetParent (joinList.transform, false);
			newServerInfo.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}

		//TODO show popup on error
	}
}
