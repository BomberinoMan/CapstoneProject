using UnityEngine;

public class UpgradeRadioactive : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerControllerComponent>().ChangePlayerControllerModifier(PlayerControllerModifierFactory.GetControllerModifier(gameObject.GetComponent<PlayerControllerComponent>().GetPlayerControllerModifier()));
    }
}
