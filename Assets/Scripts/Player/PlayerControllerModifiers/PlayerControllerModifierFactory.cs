using UnityEngine;

public static class PlayerControllerModifierFactory
{
    public static IPlayerControllerModifier getControllerModifier(IPlayerControllerModifier playerController)
    {
        int rand = UnityEngine.Random.Range(0, 7);
        switch (rand)
        {
            case 0:
                return new FastMovementPlayerControllerModifier(playerController.removeMod());
            case 1:
                return new SlowMovementPlayerControllerModifier(playerController.removeMod());
            case 2:
                return new ReverseMovementPlayerControllerModifier(playerController.removeMod());
            case 3:
                return new FastBombsPlayerControllerModifier(playerController.removeMod());
            case 4:
                return new SlowBombsPlayerControllerModifier(playerController.removeMod());
            case 5:
                return new TinyBombsPlayerControllerModifier(playerController.removeMod());
            case 6:
                return new AlwaysLayBombsPlayerControllerModifier(playerController.removeMod());
            case 7:
                return new NeverLayBombsPlayerControllerModifier(playerController.removeMod());
            default:
                Debug.Log("Applying default playerControllerModifier. Check the range of the random number genterator");
                return new DefaultPlayerControllerModifier(playerController.removeMod());
        }
    }
}
