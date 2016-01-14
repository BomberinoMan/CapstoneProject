using UnityEngine;

public static class BombManager 
{
	public static void SetupBomb(GameObject player, GameObject bomb)
	{
		BombParams temp = new BombParams ();
		BombParams playerBombParams = player.GetComponent<PlayerControllerComponent> ().bombParams;

		temp.delayTime = playerBombParams.delayTime;
		temp.explodingDuration = playerBombParams.explodingDuration;
		temp.radius = playerBombParams.radius;
		temp.warningTime = playerBombParams.warningTime;

		bomb.GetComponent<BombController> ().paramaters = temp;

		bomb.GetComponent<BombController>().parentPlayer = player;
		bomb.transform.SetParent(player.transform.parent);
	}
}
