using UnityEngine;

public static class BombManager 
{
	public static void SetupBomb(GameObject player, GameObject bomb)
	{
		bomb.GetComponent<BombController>().paramaters = player.GetComponent<PlayerController>().bombParams;
		bomb.GetComponent<BombController>().parentPlayer = player;
		bomb.transform.SetParent(player.transform.parent);
	}
}
