using UnityEngine;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour
{
	private int debug = 0;
    public GameObject background;
    public GameObject indestructible;
    public GameObject destructible;
    public GameObject upgradeBomb;
    public GameObject upgradeBombLine;
    public GameObject upgradeKick;
    public GameObject upgradeLaser;
    public GameObject upgradeRadioactive;

	public GameObject[] playerPrefab;

    public GameObject laserCross;
	public GameObject laserHor;
	public GameObject laserVert;
	public GameObject laserUp;
	public GameObject laserDown;
	public GameObject laserLeft;
	public GameObject laserRight;

    public int rows;                // 11
    public int columns;             // 13
    public float fillPercentage;    // 90
	public Coor[] playerSpawn = new Coor[4]
		{	new Coor(0, 10, null), 
			new Coor(12, 0, null), 
			new Coor(12, 0, null), 
			new Coor(12, 10, null)
		};

    //TODO get actual values for these
    public int minNumBombUpgrades;
    public int maxNumBombUpgrades;
    public int minNumLaserUpgrades;
    public int maxNumLaserUpgrades;
    public int minNumKickUpgrades;
    public int maxNumKickUpgrades;
    public int minNumLineUpgrades;
    public int maxNumLineUpgrades;

	private GameObject[] playerInstance = new GameObject[4];

    private Transform boardHolder;
	private List<GameObject> bombs = new List<GameObject>();
    private Coords indestructibleCoords = new Coords();
    private Coords destructibleCoords = new Coords();
    private Coords upgradeCoords = new Coords();

    void InitializeBoardDefault()
    {
        for (int x = -1; x <= columns; x++)
            for (int y = -1; y <= rows; y++)
            {
                // Background
                GameObject instance;

                if(x == -1 || y == -1 || x == columns || y == rows) //Border
				{
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
					indestructibleCoords.Add(x, y, instance);
				}
                else if(x % 2 == 1 && y % 2 == 1) // Rows and columns
				{
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
					indestructibleCoords.Add(x, y, instance);
				}
                else
                    instance = Instantiate(background, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
    }

    public void CreateBoard()
    {
        boardHolder = new GameObject("Board").transform;
        InitializeBoardDefault();
        //InitializeDestructible();
        InitializePlayers(1);
        //InitializeUpgrades(minNumBombUpgrades, maxNumBombUpgrades, upgradeBomb);
        //InitializeUpgrades(minNumKickUpgrades, maxNumKickUpgrades, upgradeKick);
        //InitializeUpgrades(minNumLaserUpgrades, maxNumLaserUpgrades, upgradeLaser);
        //InitializeUpgrades(minNumLineUpgrades, maxNumLineUpgrades, upgradeBombLine);
    }

    void InitializePlayers(int numPlayers)
    {
		for (int i = 0; i < numPlayers; i++) {
			playerInstance [i] = Instantiate (playerPrefab [i], new Vector3 (playerSpawn [i].x, playerSpawn [i].y, 0.0f), Quaternion.identity) as GameObject;
			InitializeSpawn (playerSpawn [i]);
			playerInstance [i].transform.SetParent (boardHolder);
		}
    }
    private void InitializeSpawn(Coor playerSpawn)
    {
        Destroy(destructibleCoords.Remove(playerSpawn));

        if (!InitializeSpawnUp(playerSpawn))
            if (!InitializeSpawnRight(playerSpawn))
                InitializeSpawnLeft(playerSpawn);

        if (!InitializeSpawnDown(playerSpawn))
            if (!InitializeSpawnLeft(playerSpawn))
                InitializeSpawnRight(playerSpawn);
    }
    private bool InitializeSpawnUp(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x, PlayerSpawn.y + 1))
            return false;

        Destroy(destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y + 1));
        Destroy(destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y + 2));

        return true;
    }
    private bool InitializeSpawnDown(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x, PlayerSpawn.y - 1))
            return false;

        Destroy(destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y - 1));
        Destroy(destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y - 2));

        return true;
    }
    private bool InitializeSpawnLeft(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x - 1, PlayerSpawn.y))
            return false;

        Destroy(destructibleCoords.Remove(PlayerSpawn.x - 1, PlayerSpawn.y));
        Destroy(destructibleCoords.Remove(PlayerSpawn.x - 2, PlayerSpawn.y));

        return true;
    }
    private bool InitializeSpawnRight(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x + 1, PlayerSpawn.y))
            return false;

        Destroy(destructibleCoords.Remove(PlayerSpawn.x + 1, PlayerSpawn.y));
        Destroy(destructibleCoords.Remove(PlayerSpawn.x + 2, PlayerSpawn.y));

        return true;
    }

    private void InitializeUpgrades(int min, int max, GameObject upgrade)
    {
        for (int i = 0; i < max; i++)
        {
            int x = (int)UnityEngine.Random.Range(0.0f, columns);
            int y = (int)UnityEngine.Random.Range(0.0f, rows);

            if (destructibleCoords.inList(x, y) && !upgradeCoords.inList(x, y))
            {
                var upgradeClone = Instantiate(upgrade, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
                upgradeCoords.Add(x, y, upgradeClone);
            }
            else
            {
                i--;
                continue;
            }

            //Stop randomly between the min and the max
            if (i >= min && UnityEngine.Random.value <= (max - min) / 100)
                break;
        }
    }

    private void InitializeDestructible()
    {
        for (int x = 0; x <= columns; x++)
            for (int y = 0; y <= rows; y++)
            {
                if(UnityEngine.Random.value <= fillPercentage && !indestructibleCoords.inList(x, y))
                {
                    GameObject instance = Instantiate(destructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
                    destructibleCoords.Add(x, y, instance);
                    instance.transform.SetParent(boardHolder);
                }
            }
    }

    public void AddBomb(GameObject bomb)
	{
		bombs.Add (bomb);
	}
	private BombParams RemoveBomb(int x, int y)
	{
		foreach (GameObject bomb in bombs) {
			if (AxisRounder.Round (bomb.transform.position.x) == x && AxisRounder.Round (bomb.transform.position.y) == y) {
				bomb.GetComponent<BombController> ().Explode (false); // Give the player another bomb
				Destroy (bomb); 	// Destroy bomb game oject
				bombs.Remove (bomb);	// Remove from list
				return bomb.GetComponent<BombController> ().paramaters;
			}
		}
		throw new InvalidProgramException ();
	}
	public bool OnBomb(int x, int y)
	{
		foreach (GameObject bomb in bombs)
			if (bomb != null && AxisRounder.Round(bomb.transform.position.x) == x && AxisRounder.Round(bomb.transform.position.y) == y)
				return true;
		return false;
	}
	public void ExplodeBomb(int x, int y)
	{
		BombParams p = RemoveBomb (x, y);	// Remove from list of bombs, destroy GameObject, give player another bomb
		GameObject instance = Instantiate (laserCross, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent (boardHolder);	// Instantiate the cross of the laser
        instance.GetComponent<LaserController>().paramaters = p;
		LaserUp (x, y, p);
		LaserDown (x, y, p);
		LaserLeft (x, y, p);
		LaserRight (x, y, p);
	}

	public bool OnPlayer (int x, int y)
	{
		foreach (GameObject player in playerInstance) {
			if (player != null && AxisRounder.Round (player.transform.position.x) == x && AxisRounder.Round (player.transform.position.y) == y)
				return true;
		}
		return false;
	}

    public void LineBomb(int x, int y, string v, int numBombs, GameObject player)
    {
		switch (v) 
		{
			case "Up":
				LineBombUp(x, y + 1, numBombs, player);
				return;
			case "Down":
				LineBombDown(x, y - 1, numBombs, player);
				return;
			case "Left":
				LineBombLeft(x - 1, y, numBombs, player);
				return;
			case "Right":
				LineBombRight(x + 1, y, numBombs, player);
				return;
			default:
				return;
		}
    }
	private void LineBombUp(int x, int y, int numBombs, GameObject player)
	{
		if (numBombs <= 0 || indestructibleCoords.inList (x, y) || destructibleCoords.inList (x, y) || OnBomb (x, y) || upgradeCoords.inList (x, y) || OnPlayer (x, y))
			return;

		GameObject bomb = Instantiate (player.GetComponent<PlayerControllerComponent>().bombObject, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		BombManager.SetupBomb (player, bomb);
		bombs.Add (bomb);

		LineBombUp (x, y + 1, numBombs - 1, player);
	}
	private void LineBombDown(int x, int y, int numBombs, GameObject player)
	{
		if (numBombs <= 0 || indestructibleCoords.inList (x, y) || destructibleCoords.inList (x, y) || OnBomb (x, y) || upgradeCoords.inList (x, y) || OnPlayer (x, y))
			return;
		Debug.Log (numBombs);
		GameObject bomb = Instantiate (player.GetComponent<PlayerControllerComponent>().bombObject, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		BombManager.SetupBomb (player, bomb);
		bombs.Add (bomb);
		
		LineBombDown (x, y - 1, numBombs - 1, player);
	}
	private void LineBombLeft(int x, int y, int numBombs, GameObject player)
	{
		if (numBombs <= 0 || indestructibleCoords.inList (x, y) || destructibleCoords.inList (x, y) || OnBomb (x, y) || upgradeCoords.inList (x, y) || OnPlayer (x, y))
			return;
		
		GameObject bomb = Instantiate (player.GetComponent<PlayerControllerComponent>().bombObject, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		BombManager.SetupBomb (player, bomb);
		bombs.Add (bomb);
		
		LineBombLeft (x - 1, y, numBombs - 1, player);
	}
	private void LineBombRight(int x, int y, int numBombs, GameObject player)
	{
		if (numBombs <= 0 || indestructibleCoords.inList (x, y) || destructibleCoords.inList (x, y) || OnBomb (x, y) || upgradeCoords.inList (x, y) || OnPlayer (x, y))
			return;
		
		GameObject bomb = Instantiate (player.GetComponent<PlayerControllerComponent>().bombObject, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		BombManager.SetupBomb (player, bomb);
		bombs.Add (bomb);
		
		LineBombRight (x + 1, y, numBombs - 1, player);
	}
    private void LaserUp(int x, int y, BombParams p)
	{
		GameObject laser;
		for (int i = 0; i <= p.radius; i++) {
			y++;
			if(OnBomb(x, y))
			{
			   	ExplodeBomb(x, y);
				continue;
			}
			else if(indestructibleCoords.inList(x, y))
			{
				break;
			}
            else if(destructibleCoords.inList(x, y))
            {
                Destroy(destructibleCoords.Remove(x, y));
                break;
            }
            else if(upgradeCoords.inList(x, y))
            {
                Destroy(upgradeCoords.Remove(x, y));
            }
			if(i != p.radius)
				laser = Instantiate(laserVert, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserUp, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
            laser.GetComponent<LaserController>().paramaters = p;

            laser.transform.SetParent(boardHolder);
		}
	}
	private void LaserDown(int x, int y, BombParams p)
	{
		GameObject laser;
		for (int i = 0; i <= p.radius; i++) {
			y--;
			if(OnBomb(x, y))
			{
				ExplodeBomb(x, y);
				continue;
			}
			else if(indestructibleCoords.inList(x, y))
			{
				break;
			}
            else if (destructibleCoords.inList(x, y))
            {
                Destroy(destructibleCoords.Remove(x, y));
                break;
            }
            else if (upgradeCoords.inList(x, y))
            {
                Destroy(upgradeCoords.Remove(x, y));
            }
            if (i != p.radius)
				laser = Instantiate(laserVert, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserDown, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
            laser.GetComponent<LaserController>().paramaters = p;

            laser.transform.SetParent(boardHolder);
		}
	}
	private void LaserLeft(int x, int y, BombParams p)
	{
		GameObject laser;
		for (int i = 0; i <= p.radius; i++) {
			x--;
			if(OnBomb(x, y))
			{
				ExplodeBomb(x, y);
				continue;
			}
			else if(indestructibleCoords.inList(x, y))
			{
				break;
			}
            else if (destructibleCoords.inList(x, y))
            {
                Destroy(destructibleCoords.Remove(x, y));
                break;
            }
            else if (upgradeCoords.inList(x, y))
            {
                Destroy(upgradeCoords.Remove(x, y));
            }
            if (i != p.radius)
				laser = Instantiate(laserHor, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserLeft, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
            laser.GetComponent<LaserController>().paramaters = p;

            laser.transform.SetParent(boardHolder);
		}
	}
	private void LaserRight(int x, int y, BombParams p)
	{
		GameObject laser;
		for (int i = 0; i <= p.radius; i++) {
			x++;
			if(OnBomb(x, y))
			{
				ExplodeBomb(x, y);
				continue;
			}
			else if(indestructibleCoords.inList(x, y))
			{
				break;
			}
            else if (destructibleCoords.inList(x, y))
            {
                Destroy(destructibleCoords.Remove(x, y));
                break;
            }
            else if (upgradeCoords.inList(x, y))
            {
                Destroy(upgradeCoords.Remove(x, y));
            }
            if (i != p.radius)
				laser = Instantiate(laserHor, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserRight, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
            laser.GetComponent<LaserController>().paramaters = p;

            laser.transform.SetParent(boardHolder);
		}
	}
}
