public static class BoardParamsFactory
{

    public static BoardParams GetBoardParams(int i = 0)
    {
        switch (i)
        {
            case 0:
            default:
                return new BoardParams
                {
                    fillPercentage = 0.9f,
                    minNumBombUpgrades = 10,
                    maxNumBombUpgrades = 15,
                    minNumLaserUpgrades = 10,
                    maxNumLaserUpgrades = 15,
                    minNumKickUpgrades = 1,
                    maxNumKickUpgrades = 3,
                    minNumLineUpgrades = 1,
                    maxNumLineUpgrades = 1,
                    minNumRadioactiveUpgrades = 5,
                    maxNumRadioactiveUpgrades = 10
                };
        }
    }
}
