using UnityEngine;

public class PlayerController : IPlayerController
{
    public float speedScalar { get; set; }
    public int bombKick { get; set; }
    public int bombLine { get; set; }
    public int currNumBombs { get; set; }
    private int _maxNumBombs;
    public int maxNumBombs
    {
        get { return _maxNumBombs; }
        set
        {
            _maxNumBombs = value;
            currNumBombs++;
        }
    }
    public BombParams bombParams { get; set; }
    public bool canLayBombs { get; set; }
    public bool alwaysLayBombs { get; set; }
    public bool reverseMovement { get; set; }
    public bool isRadioactive { get; set; }
}
