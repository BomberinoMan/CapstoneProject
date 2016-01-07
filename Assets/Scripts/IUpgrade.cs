using System;
using UnityEngine;

public interface IUpgrade
{
    void ApplyEffect(GameObject gameObject);
}

public class UpgradeBomb : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerController>();

        playerController.maxNumBombs++;
    }
}

public class UpgradeLaser : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerController>();

        playerController.bombParams.radius++;
    }
}

public class UpgradeKick : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerController>();

        playerController.bombKick++;
    }
}

public class UpgradeBombLine : IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        var playerController = gameObject.GetComponent<PlayerController>();

        playerController.bombLine++;
    }
}

//TODO add the code for all of the radioactive effects here aswell

public class UpgradeTestRadioactive : MonoBehaviour, IUpgrade
{
    public void ApplyEffect(GameObject gameObject)
    {
        throw new NotImplementedException();
    }
}
