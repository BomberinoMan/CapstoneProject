using UnityEngine;

public class UpgradeRadioactive : IUpgrade {

	public void ApplyEffect(GameObject gameObject)
	{
		gameObject.GetComponent<PlayerControllerComponent> ().changePlayerControllerModifier (PlayerControllerModifierFactory.getControllerModifier(gameObject.GetComponent<PlayerControllerComponent> ().getPlayerControllerModifier ().removeMod ()));
	}
}
