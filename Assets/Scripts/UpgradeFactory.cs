using UnityEngine;

public enum Upgrades
{
    Bomb,
    Laser,
    Line,
    Kick,
    Radioactive
}

public static class UpgradeFactory
{
    public static IUpgrade getUpgrade(Upgrades upgrade)
    {
        switch(upgrade)
        {
            case Upgrades.Bomb:
                return new UpgradeBomb();
            case Upgrades.Laser:
                return new UpgradeLaser();
            case Upgrades.Line:
                return new UpgradeBombLine();
            case Upgrades.Kick:
                return new UpgradeKick();
            case Upgrades.Radioactive:
                //TODO implement support for radioactive upgrades                
            default:
                Debug.LogError(upgrade + " is not a supported upgrade type");
                return null;                
        }
    }
}
