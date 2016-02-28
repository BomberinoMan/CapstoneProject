using UnityEngine;

public class SlowBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private BombParams temp;

    public SlowBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
        temp = new BombParams();

        temp.delayTime = _playerController.bombParams.delayTime;
        temp.explodingDuration = _playerController.bombParams.explodingDuration;
        temp.radius = _playerController.bombParams.radius;
        temp.warningTime = _playerController.bombParams.warningTime;

        temp.warningTime = temp.warningTime * 8.0f;
    }

    public override BombParams bombParams
    {
        get
        {
            if (!isRadioactive)
                return _playerController.bombParams;

            return temp;
        }
        set
        {
            temp.explodingDuration = value.explodingDuration;
            temp.radius = value.radius;
            temp.warningTime = value.warningTime;
            _playerController.bombParams = value;
        }
    }
}