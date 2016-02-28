using UnityEngine;

public class UpgradeBomb : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerControllerComponent>();

        playerController.maxNumBombs++;
    }
}