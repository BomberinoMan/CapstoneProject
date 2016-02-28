using UnityEngine;

public class NeverLayBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    public NeverLayBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
    }

    public override bool canLayBombs
    {
        get
        {
            return !isRadioactive;
        }
        set
        {
            Debug.Log("Cannot change neverLayBombs");
            //Do nothing		
        }
    }
}
