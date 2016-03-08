using UnityEngine;
using UnityEngine.Networking;

public class LaserController : NetworkBehaviour
{
    public BombParams paramaters;
    public float creationTime;

    void Update()
    {
        if (paramaters == null)
            return;
        if (creationTime + paramaters.explodingDuration <= Time.time)
            Destroy(gameObject);
    }

}
