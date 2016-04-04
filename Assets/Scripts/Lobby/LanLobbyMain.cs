using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

public class LanLobbyMain : MonoBehaviour
{
    public Button hostButton;
    public Button backButton;
    public Button refreshButton;
    public RectTransform joinList;
    public GameObject serverInfo;

    private bool _isResponse;
    private GeneralResponse _response;
    private Action<GeneralResponse> _responseCallback;

    public void OnEnable()
    {
        _isResponse = false;
        _responseCallback = null;

        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(OnClickHost);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);

        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(OnClickRefresh);

        OnClickRefresh();
    }

    void Update()
    {
        if (_isResponse)
        {
            if (_responseCallback != null)
            {
                _responseCallback(_response);
            }
            _isResponse = false;
        }
    }

    public void OnClickHost()
    {
        LobbyManager.instance.DisplayInfoInputField("Room Name", OnSubmitHostName);
    }

    public void OnSubmitHostName(string name)
    {
        _isResponse = false;
        _responseCallback = new Action<GeneralResponse>(CreateRoomCallback);
        new Thread(() => CreateRoom(name)).Start();
    }

    public void CreateRoom(string roomname)
    {
        _response = DBConnection.instance.CreateRoom(new CreateRoomMessage { userId = LoginInformation.guid, name = roomname });
        _isResponse = true;
    }

    public void CreateRoomCallback(GeneralResponse response)
    {
        var createResponse = (CreateRoomResponse)response;

        if (createResponse.isSuccessful)
        {
            LobbyManager.instance.StartHost();
        }
    }

    public void OnClickBack()
    {
        Destroy(LobbyManager.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickRefresh()
    {
        for (int i = joinList.childCount - 1; i >= 0; i--)
        {
            Destroy(joinList.GetChild(i).gameObject);
        }

        _isResponse = false;
        _responseCallback = new Action<GeneralResponse>(ListRoomsCallback);
        new Thread(ListRooms).Start();

        //TODO show popup on error
    }

    public void ListRooms()
    {
        _response = DBConnection.instance.ListRooms(new ListRoomsMessage { userId = LoginInformation.guid });
        _isResponse = true;
    }

    public void ListRoomsCallback(GeneralResponse response)
    {
        var listResponse = (ListRoomsResponse)response;

        if (listResponse.isSuccessful)
            foreach (var room in listResponse.rooms)
            {
                var newServerInfo = Instantiate(serverInfo);

                newServerInfo.GetComponent<LobbyServerInfo>().PopulateMatchInfo(room.name, room.ip);
                newServerInfo.transform.SetParent(joinList.transform, false);
                newServerInfo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

        if (listResponse.rooms.Length == 0)
        {
            var newServerInfo = Instantiate(serverInfo);

            newServerInfo.GetComponent<LobbyServerInfo>().PopulateMatchInfo("No games exist", "", false);
            newServerInfo.transform.SetParent(joinList.transform, false);
            newServerInfo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
