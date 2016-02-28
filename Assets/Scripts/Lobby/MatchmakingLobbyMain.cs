﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Match;

public class MatchmakingLobbyMain : MonoBehaviour
{
    public InputField roomName;
    public Button createButton;
    public Button findButton;
    public Button backButton;
    public RectTransform listPanel;
    public GameObject serverInfoPrefab;

    public void OnEnable()
    {
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(OnClickCreate);

        findButton.onClick.RemoveAllListeners();
        findButton.onClick.AddListener(OnClickFind);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void OnClickCreate()
    {
        var maxPlayers = (uint)LobbyManager.instance.maxPlayers;
        LobbyManager.instance.StartMatchMaker();
        LobbyManager.instance.matchMaker.CreateMatch(roomName.text, maxPlayers, true, "", LobbyManager.instance.OnMatchCreate);
    }

    public void OnClickFind()
    {
        LobbyManager.instance.StartMatchMaker();
        //LobbyManager._instance.ChangePanel(LobbyManager._instance.lobbyServerList);
        LobbyManager.instance.matchMaker.ListMatches(0, 10, "", OnMatchList);
    }

    private void OnMatchList(ListMatchResponse response)
    {
        Debug.Log("MATCH COUNT: " + response.matches.Count);

        foreach (MatchDesc match in response.matches)
        {
            GameObject go = Instantiate(serverInfoPrefab) as GameObject;
            go.GetComponent<LobbyServerInfo>().PopulateMatchInfo(match);
            go.transform.SetParent(listPanel, false);
        }
    }

    public void OnClickBack()
    {
        Destroy(LobbyManager.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
