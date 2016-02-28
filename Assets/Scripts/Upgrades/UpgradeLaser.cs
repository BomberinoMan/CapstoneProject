using UnityEngine;

public class UpgradeLaser : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerControllerComponent>();

        playerController.bombParams.radius++;
    }
}