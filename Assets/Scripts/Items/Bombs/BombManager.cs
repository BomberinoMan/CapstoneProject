using UnityEngine;

public static class BombManager 
{
	public static void SetupBomb(GameObject player, GameObject bomb, bool setupColliders = true)
    {
        BombParams temp = new BombParams();
        BombParams playerBombParams = player.GetComponent<PlayerControllerComponent>().bombParams;

        temp.delayTime = playerBombParams.delayTime;
        temp.explodingDuration = playerBombParams.explodingDuration;
        temp.radius = playerBombParams.radius;
        temp.warningTime = playerBombParams.warningTime;

        bomb.GetComponent<BombController>().paramaters = temp;

        bomb.GetComponent<BombController>().parentPlayer = player;
        bomb.transform.SetParent(player.transform.parent);

		if(setupColliders)
	        foreach (Collider2D collider in bomb.GetComponentsInChildren<Collider2D>())   // Ignore collisions on all child colliders aswell, do not ignore colliders that are triggers
	            if (!collider.isTrigger)
	                Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>());
    }
}
