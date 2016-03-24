using UnityEngine;

public static class PlayerControllerModifierFactory
{
    public static IPlayerControllerModifier GetControllerModifier(IPlayerControllerModifier playerController)
    {
        int rand = Random.Range(0, 7);
        switch (rand)
        {
            case 0:
                return new FastMovementPlayerControllerModifier(playerController.RemoveMod());
            case 1:
                return new SlowMovementPlayerControllerModifier(playerController.RemoveMod());
            case 2:
                return new ReverseMovementPlayerControllerModifier(playerController.RemoveMod());
            case 3:
                return new FastBombsPlayerControllerModifier(playerController.RemoveMod());
            case 4:
                return new SlowBombsPlayerControllerModifier(playerController.RemoveMod());
            case 5:
                return new TinyBombsPlayerControllerModifier(playerController.RemoveMod());
            case 6:
                return new AlwaysLayBombsPlayerControllerModifier(playerController.RemoveMod());
            case 7:
                return new NeverLayBombsPlayerControllerModifier(playerController.RemoveMod());
            default:
                Debug.LogError("Applying default playerControllerModifier. Check the range of the random number genterator");
                return new DefaultPlayerControllerModifier(playerController.RemoveMod());
        }
    }
}
