using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TouchBomb : NetworkBehaviour, IPointerDownHandler
{
    private PlayerControllerComponent player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.TouchLayBomb();
    }

    public void SetPlayerController(PlayerControllerComponent playerController)
    {
        player = playerController;
    }
}
