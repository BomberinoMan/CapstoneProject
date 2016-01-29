using UnityEngine;

public enum UpgradeType
{
	None,
    Bomb,
    Laser,
    Line,
    Kick,
    Radioactive
}

public static class UpgradeFactory
{
    public static IUpgrade getUpgrade(UpgradeType upgrade)
    {
        switch(upgrade)
        {
        case UpgradeType.Bomb:
            return new UpgradeBomb();
        case UpgradeType.Laser:
            return new UpgradeLaser();
        case UpgradeType.Line:
            return new UpgradeBombLine();
        case UpgradeType.Kick:
            return new UpgradeKick();
		case UpgradeType.Radioactive:
			return new UpgradeRadioactive ();   
		case UpgradeType.None:
        default:
            Debug.LogError(upgrade + " is not a supported upgrade type");
            return null;                
        }
    }
}
