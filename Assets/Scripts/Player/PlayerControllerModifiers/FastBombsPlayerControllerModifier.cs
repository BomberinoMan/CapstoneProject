using UnityEngine;

public class FastBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private BombParams temp;

    public FastBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
        temp = new BombParams();

        temp.delayTime = _playerController.bombParams.delayTime;
        temp.explodingDuration = _playerController.bombParams.explodingDuration;
        temp.radius = _playerController.bombParams.radius;
        temp.warningTime = _playerController.bombParams.warningTime;

        temp.delayTime = temp.delayTime / 4.0f;
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