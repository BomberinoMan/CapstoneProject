using UnityEngine;

public class FastBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private BombParams _temp;

    public FastBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
        _temp = new BombParams();

        _temp.delayTime = _playerController.bombParams.delayTime;
        _temp.explodingDuration = _playerController.bombParams.explodingDuration;
        _temp.radius = _playerController.bombParams.radius;
        _temp.warningTime = _playerController.bombParams.warningTime;

        _temp.delayTime = _temp.delayTime / 4.0f;
    }

    public override BombParams bombParams
    {
        get
        {
            if (!isRadioactive)
                return _playerController.bombParams;

            return _temp;
        }
        set
        {
            _temp.explodingDuration = value.explodingDuration;
            _temp.radius = value.radius;
            _temp.warningTime = value.warningTime;
            _playerController.bombParams = value;
        }
    }
}