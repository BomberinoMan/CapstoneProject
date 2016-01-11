public interface IPlayerController{
	float speedScalar { get; set; }
	int bombKick { get; set; }
	int bombLine { get; set; }
	int currNumBombs { get; set; }
	int maxNumBombs { get;set;}
	BombParams bombParams { get; set; }
	bool canLayBombs { get; set; }
	bool alwaysLayBombs { get; set; }
	bool reverseMovement { get; set; }
	bool isRadioactive { get; set; }
}
