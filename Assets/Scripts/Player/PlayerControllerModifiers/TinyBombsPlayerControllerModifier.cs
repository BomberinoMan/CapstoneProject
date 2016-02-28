using UnityEngine;

public class TinyBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private BombParams _temp;

    public TinyBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
        _temp = new BombParams();

        _temp.delayTime = _playerController.bombParams.delayTime;
        _temp.explodingDuration = _playerController.bombParams.explodingDuration;
        _temp.radius = 1;
        _temp.warningTime = _playerController.bombParams.warningTime;
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
            _temp.warningTime = value.warningTime;
            _temp.delayTime = value.delayTime;
            _playerController.bombParams = value;
        }
    }
}
