using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuArea : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("MENUBUTTONPRESSED");
        LobbyManager.instance.ShowInGameMenu();
    }
}
