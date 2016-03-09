using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine;

public class TouchBomb : MonoBehaviour, IPointerDownHandler
{
    private PlayerControllerComponent _player;

    public void OnPointerDown(PointerEventData eventData)
    {
        _player.TouchLayBomb();
    }

    public void SetPlayerController(PlayerControllerComponent playerController)
    {
        _player = playerController;
    }
}
