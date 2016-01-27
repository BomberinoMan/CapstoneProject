using UnityEngine;
using System.Collections;

public class NeverLayBombsPlayerControllerModifier : DefaultPlayerControllerModifier
{
    private float startTime;
    private IPlayerController _playerController;

    public NeverLayBombsPlayerControllerModifier(IPlayerController playerController)
    {
        startTime = Time.time;
        //_playerController = playerController;
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

    public override bool isRadioactive { get { return startTime + duration >= Time.time; } }
}
