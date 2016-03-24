using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine;

public class TouchBomb : MonoBehaviour, IPointerDownHandler
{
    private PlayerControllerComponent _player;

    public void OnPointerDown(PointerEventData eventData)
    {
		if (_player != null)
	        _player.TouchLayBomb();
    }

    public void SetPlayerController(PlayerControllerComponent playerController)
    {
        _player = playerController;
    }

	public void Update(){
		if (Input.GetKeyDown (KeyCode.Space) && _player != null)
			_player.TouchLayBomb ();
	}
}
