using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : NetworkBehaviour {
	public static GameManager instance;
	public float scoreScreenTime = 5.0f;
	private bool _isGameOver = false;
	private BoardCreator _boardCreator;

	// **** Player ****
	public GameObject[] playerAnimations;
	private Vector3[] _playerSpawnVectors = new Vector3[4]
	{
		new Vector3(1.0f, 11.0f, 0.0f),
		new Vector3(13.0f, 1.0f, 0.0f),
		new Vector3(13.0f, 11.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f)
	};
	public Vector3[] playerSpawnVectors { get { return _playerSpawnVectors; } }

	// **** Tiles ****
	public GameObject bombUpgrade;
	public GameObject laserUpgrade;
	public GameObject kickUpgrade;
	public GameObject lineUpgrade;
	public GameObject radioactiveUpgrade;
	public GameObject floor;
	public GameObject destructible;
	public GameObject indestructible;

	void Awake(){
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	public void PlayerDead(PlayerControllerComponent player)
	{
		if (!isServer)
			return;
		
		SpawnUpgradeInRandomLocation(UpgradeType.Bomb, player.maxNumBombs - 1);
		SpawnUpgradeInRandomLocation(UpgradeType.Laser, player.bombParams.radius - 2);
		SpawnUpgradeInRandomLocation(UpgradeType.Kick, player.bombKick);
		SpawnUpgradeInRandomLocation(UpgradeType.Line, player.bombLine);

		(LobbyManager.instance.lobbySlots.Where (x => x.slot == player.slot).First () as LobbyPlayer).isAlive = false;

		Invoke("CheckIfGameOver", 2);

		NetworkServer.Destroy (player.gameObject);
	}

	private void CheckIfGameOver()
	{
		if (LobbyManager.instance.lobbySlots.Where(x => x != null).Select(x => x as LobbyPlayer).Where(x => x.isAlive).Count() == 1)
		{
			LobbyManager.instance.lobbySlots.Where(x => x != null).Select(x => x as LobbyPlayer).Where(x => x.isAlive).First().score++;
			LobbyManager.instance.lobbySlots.Where(x => x != null).Select(x => x as LobbyPlayer).Where(x => x.isAlive).First().isAlive = false;
		}

		if (LobbyManager.instance.lobbySlots.Where(x => x != null).Select(x => x as LobbyPlayer).Where(x => x.isAlive).Where(x => x.isAlive).Count() == 0)
		{
			StartCoroutine(GameOver());
		}
	}

	private IEnumerator GameOver()
	{
		if (_isGameOver)
			yield break;

		_isGameOver = true;
		float remainingTime = scoreScreenTime;

		foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
		{
			if (player == null)
				continue;

			player.RpcAddPlayerToScoreList(player.GetUsername(), player.score);
		}

		while (remainingTime >= -1)
		{
			yield return null;
			remainingTime -= Time.deltaTime;
		}

		foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
		{
			if (player == null)
				continue;

			player.RpcClearScoreList();
		}

		_isGameOver = false;
		LobbyManager.instance.GameIsOver ();
	}

	public void SpawnBoard()
	{
		if (!isServer)
			return;
		_boardCreator = new BoardCreator();
		_boardCreator.InitializeDestructible();

		//Initialize spawn for all connected players
		LobbyManager.instance.lobbySlots.Where(p => p != null).ToList()
			.ForEach(p => _boardCreator.InitializeSpawn(_playerSpawnVectors[p.slot]));

		//Initialize all upgrades
		_boardCreator.InitializeUpgrades();

		//Get the generated tiles in the board
		var board = _boardCreator.GetBoard();

		//Spawn all objects in the board
		foreach (var tile in board.tiles)
		{
			if (tile.isIndestructible)
			{
				NetworkServer.Spawn(Instantiate(indestructible, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				continue;
			}

			NetworkServer.Spawn(Instantiate(floor, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);

			if (tile.isDestructible)
				NetworkServer.Spawn(Instantiate(destructible, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);

			if (tile.isUpgrade)
				switch (tile.upgradeType)
			{
			case (UpgradeType.Bomb):
				NetworkServer.Spawn(Instantiate(bombUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Kick):
				NetworkServer.Spawn(Instantiate(kickUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Laser):
				NetworkServer.Spawn(Instantiate(laserUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Line):
				NetworkServer.Spawn(Instantiate(lineUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Radioactive):
				NetworkServer.Spawn(Instantiate(radioactiveUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
				break;
			default: 
				// Do nothing
				break;
			}
		}
	}

	private void SpawnUpgradeInRandomLocation(UpgradeType upgradeType, int num = 0)
	{
		for (int i = 0; i < num; i++)
		{
			Vector2 location = new Vector2();

			do
			{
				// Spawnable locations on the board
				location.x = UnityEngine.Random.Range(1, 13);   
				location.y = UnityEngine.Random.Range(1, 11);
			} while (Physics2D.RaycastAll(location, new Vector2(1.0f, 1.0f), 0.2f).Length != 0);

			switch (upgradeType)
			{
			case (UpgradeType.Bomb):
				NetworkServer.Spawn(Instantiate(bombUpgrade, location, Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Kick):
				NetworkServer.Spawn(Instantiate(kickUpgrade, location, Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Laser):
				NetworkServer.Spawn(Instantiate(laserUpgrade, location, Quaternion.identity) as GameObject);
				break;
			case (UpgradeType.Line):
				NetworkServer.Spawn(Instantiate(lineUpgrade, location, Quaternion.identity) as GameObject);
				break;
			default: // Do nothing
				break;
			}
		}
	}
}
