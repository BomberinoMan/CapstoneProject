using UnityEngine;

public class SlowMovementPlayerControllerModifier : DefaultPlayerControllerModifier
{
    public SlowMovementPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
    }

    public override float speedScalar
    {
        get
        {
            if (isRadioactive)
                return 0.5f;
            return _playerController.speedScalar;
        }
        set { _playerController.speedScalar = value; }
    }
}
