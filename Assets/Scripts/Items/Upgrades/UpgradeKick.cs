using UnityEngine;

public class UpgradeKick : IUpgrade
{
	public void ApplyEffect(GameObject gameObject)
	{
		var playerController = gameObject.GetComponent<PlayerControllerComponent>();

		playerController.bombKick++;
	}
}