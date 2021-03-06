﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public Button resumeButton;
    public Button scoreButton;
    public Button quitButton;

    public void OnEnable()
    {
        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(OnClickResume);

        scoreButton.onClick.RemoveAllListeners();
        scoreButton.onClick.AddListener(OnClickScore);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(OnClickQuit);
    }

    public void OnClickResume()
    {
        LobbyManager.instance.HideInGameMenu();
    }

    public void OnClickScore()
    {
        LobbyManager.instance.ShowScorePanel();
        LobbyManager.instance.scoreScreen.UpdateScoreList();
    }

    public void OnClickQuit()
    {
        LobbyManager.instance.localPlayer.Leave();
        LobbyManager.instance.HideInGameMenu();
    }
}
