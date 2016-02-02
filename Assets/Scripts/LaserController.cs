using UnityEngine;
using UnityEngine.Networking;

public class LaserController : NetworkBehaviour {
    public BombParams paramaters;
	public float creationTime;

	[ClientRpc]
	public void RpcSetupLaser(float creationTime, BombParams paramaters)
	{
		creationTime = Time.time;
		paramaters = paramaters;
	}

    void Update()
    {
		if (paramaters == null)
			return;
        if (creationTime + paramaters.explodingDuration <= Time.time)
            Destroy(gameObject);
    }

}
