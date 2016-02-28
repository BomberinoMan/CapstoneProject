using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TouchDirection : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DPadController controller;
    public Vector2 Direction;

    void Start()
    {
        controller = GetComponentInParent<DPadController>();
    }

    // TODO These are only here as placeholders until we find a decent way to control the player with touch controls
    public void OnPointerEnter(PointerEventData eventData)
    {
        //controller.currDirection = Direction;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (controller.currDirection == Direction)
        //   controller.currDirection = Vector2.zero;
    }
}
