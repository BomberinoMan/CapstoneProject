using UnityEngine;

public class UpgradeBombLine : IUpgrade
{
	public void ApplyEffect(GameObject gameObject)
	{
		var playerController = gameObject.GetComponent<PlayerControllerComponent>();

		playerController.bombLine++;
	}
}