using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

public class LobbyUIManager {
	/*
	private static LobbyUIManager _instance;
	public static LobbyUIManager instance{ 
		get { 
			if (_instance == null)
				_instance = new LobbyUIManager ();
			return _instance;
		}
	}

	public RectTransform scoreScreenGui;
	public RectTransform lobbyGui;
	public RectTransform menuGui;
	public RectTransform infoGui;
	public RectTransform inGameMenu;
	public Text infoText;
	public Button infoButton;
	public float countdownTime = 5.0f;

	private RectTransform _currentPanel;


	public void DisplayInfoPanel(string message, UnityAction onCancel){
		infoText.text = message;
		infoButton.onClick.RemoveAllListeners();
		infoButton.onClick.AddListener(onCancel);

		infoButton.gameObject.SetActive(true);
		infoGui.gameObject.SetActive(true);
	}

	public void HideInfoPanel()
	{
		infoGui.gameObject.SetActive(false);
	}

	public void DisableAllPanels(){
		scoreScreenGui.gameObject.SetActive (false);
		lobbyGui.gameObject.SetActive (false);
		menuGui.gameObject.SetActive (false);
		infoGui.gameObject.SetActive (false);
		inGameMenu.gameObject.SetActive (false);
	}

	public void ChangePanel(RectTransform newPanel){
//		if (scoreScreenGui == null)
//			return;
		scoreScreenGui.gameObject.SetActive (newPanel == scoreScreenGui);
		lobbyGui.gameObject.SetActive (newPanel == lobbyGui);
		menuGui.gameObject.SetActive (newPanel == menuGui);
		infoGui.gameObject.SetActive (newPanel == infoGui);
		inGameMenu.gameObject.SetActive (newPanel == inGameMenu);
	}

	public void EnablePanel(RectTransform panelToEnable){
		panelToEnable.gameObject.SetActive (true);
	}

	public void DisablePanel(RectTransform panelToDisable){
		panelToDisable.gameObject.SetActive (false);
	}
	*/
}
