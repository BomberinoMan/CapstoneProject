using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TouchBomb : NetworkBehaviour, IPointerDownHandler
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
