using UnityEngine;

public class UpgradeRadioactive : IUpgrade
{

    public void ApplyEffect(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerControllerComponent>().ChangePlayerControllerModifier(PlayerControllerModifierFactory.getControllerModifier(gameObject.GetComponent<PlayerControllerComponent>().GetPlayerControllerModifier()));
    }
}
