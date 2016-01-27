public interface IPlayerControllerModifier : IPlayerController {
	bool isRadioactive { get; set; }
	IPlayerController removeMod();
}
