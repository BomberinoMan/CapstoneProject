using UnityEngine;

public class ReverseMovementPlayerControllerModifier : DefaultPlayerControllerModifier
{
    public ReverseMovementPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
    }

    public override bool reverseMovement
    {
        get
        {
            return isRadioactive;
        }
        set { _playerController.reverseMovement = value; }
    }
}
