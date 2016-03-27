using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    public GameObject playerScoreInfoPrefab;
    public RectTransform scoreListPanel;
    public Button closeButton;

    public void OnEnable()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);
    }

    void FixedUpdate()
    {
        UpdateScoreList();
    }

    public void OnClickClose()
    {
        LobbyManager.instance.HideScorePanel();
    }

    public void UpdateScoreList()
    {
        ClearScoreList();
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                GameObject go = Instantiate(playerScoreInfoPrefab) as GameObject;
                go.GetComponent<PlayerScoreInfo>().PopulateScoreInfo(player.GetUsername(), player.score);
                go.transform.SetParent(scoreListPanel, false);
                go.SetActive(true);
            }
        }
    }

    public void ClearScoreList()
    {
        foreach (Transform scoreInfo in scoreListPanel)
        {
            Destroy(scoreInfo.gameObject);
        }
    }
}
