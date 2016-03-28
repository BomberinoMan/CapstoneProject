using UnityEngine;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour
{
    public GameObject lobbyPlayerInfoPrefab;
    public RectTransform playerList;
    public Button leaveButton;

    public void OnEnable()
    {
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        ClearPlayerList();
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                GameObject go = Instantiate(lobbyPlayerInfoPrefab) as GameObject;
                go.GetComponent<LobbyPlayerInfo>().PopulatePlayerInfo(player.GetUsername(), player.GetPosition(), player.readyToBegin);
                go.transform.SetParent(playerList, false);
                go.SetActive(true);

                SetupPlayerListeners(player, go.GetComponent<LobbyPlayerInfo>().readyButton, leaveButton);
            }
        }
    }

    public void ClearPlayerList()
    {
        foreach (Transform playerInfo in playerList)
        {
            Destroy(playerInfo.gameObject);
        }
    }

    public void SetupPlayerListeners(LobbyPlayer player, Button ready, Button leave)
    {
        player.readyButton = ready;
        player.leaveButton = leave;

        if (player.isLocalPlayer)
        {
            SetupLocalPlayer(player, ready, leave);
        }
        else
        {
            SetupRemotePlayer(ready);
        }
    }

    public void SetupLocalPlayer(LobbyPlayer player, Button ready, Button leave)
    {
        ready.interactable = true;
        ready.onClick.RemoveAllListeners();
        ready.onClick.AddListener(OnClickReady);

        leave.interactable = true;
        leave.onClick.RemoveAllListeners();
        leave.onClick.AddListener(OnClickLeave);
    }

    public void SetupRemotePlayer(Button ready)
    {
        ready.interactable = false;
    }

    public void OnClickReady()
    {
        LobbyManager.instance.localPlayer.ToggleReady();
    }

    public void OnClickLeave()
    {
        LobbyManager.instance.localPlayer.Leave();
    }
}
