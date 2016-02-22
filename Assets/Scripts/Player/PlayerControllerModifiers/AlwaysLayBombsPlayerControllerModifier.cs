using UnityEngine;
using System.Collections;

public class AlwaysLayBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    public AlwaysLayBombsPlayerControllerModifier(IPlayerController playerController)
    {
        _startTime = Time.time;
        _playerController = playerController;
    }

    public override bool alwaysLayBombs
    {
        get
        {
            return isRadioactive;
        }
        set
        {
            Debug.Log("Cannot change alwaysLayBombs");
            //Do nothing		
        }
    }
}
