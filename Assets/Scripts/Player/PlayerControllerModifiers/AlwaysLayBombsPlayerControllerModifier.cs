using UnityEngine;
using System.Collections;

public class AlwaysLayBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private float startTime;
    private IPlayerController _playerController;

    public AlwaysLayBombsPlayerControllerModifier(IPlayerController playerController)
    {
        startTime = Time.time;
        //_playerController = playerController;
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

    public override bool isRadioactive { get { return startTime + duration >= Time.time; } }
}
