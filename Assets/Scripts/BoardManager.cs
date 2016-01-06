using UnityEngine;
using System.Collections.Generic;
using System;

struct BombCoords{
	public GameObject bomb;
	public int x;
	public int y;
	public BombParams p;

	public BombCoords(GameObject bomb, int x, int y, BombParams p)
	{
		this.bomb = bomb;
		this.x = x;
		this.y = y;
		this.p = p;
	}
}

public class BoardManager : MonoBehaviour {

    public GameObject background;
    public GameObject indestructible;
    public GameObject destructible;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

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
    public Coor player1Spawn = new Coor(0, 10, null);     // 0,10
    public Coor player2Spawn = new Coor(12, 0, null);     // 12,0
    public Coor player3Spawn = new Coor(0, 0, null);     // 0,0
    public Coor player4Spawn = new Coor(12, 10, null);     // 12,10

    private Transform boardHolder;
	private List<BombCoords> bombs = new List<BombCoords>();
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
        InitializeDestructible();
        //InitializeUpgrades();
        InitializePlayers(2);
    }

    void InitializePlayers(int numPlayers)
    {
        switch(numPlayers)
        {
            case 4:
                GameObject player4Instance = Instantiate(player4, new Vector3(player4Spawn.x, player4Spawn.y, 0.0f), Quaternion.identity) as GameObject;
                InitializeSpawn(player4Spawn);

                player4Instance.transform.SetParent(boardHolder);
                goto case 3;
            case 3:
                GameObject player3Instance = Instantiate(player3, new Vector3(player3Spawn.x, player3Spawn.y, 0.0f), Quaternion.identity) as GameObject;
                InitializeSpawn(player3Spawn);

                player3Instance.transform.SetParent(boardHolder);
                goto case 2;
            case 2:
                GameObject player2Instance = Instantiate(player2, new Vector3(player2Spawn.x, player2Spawn.y, 0.0f), Quaternion.identity) as GameObject;
                InitializeSpawn(player2Spawn);

                player2Instance.transform.SetParent(boardHolder);
                goto case 1;
            case 1:
                GameObject player1Instance = Instantiate(player1, new Vector3(player1Spawn.x, player1Spawn.y, 0.0f), Quaternion.identity) as GameObject;
                InitializeSpawn(player1Spawn);

                player1Instance.transform.SetParent(boardHolder);
                break;
            default:
                break;
        }
    }
    private void InitializeSpawn(Coor playerSpawn)
    {
        destructibleCoords.Remove(playerSpawn);

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

        destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y + 1);
        destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y + 2);

        return true;
    }
    private bool InitializeSpawnDown(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x, PlayerSpawn.y - 1))
            return false;

        destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y - 1);
        destructibleCoords.Remove(PlayerSpawn.x, PlayerSpawn.y - 2);

        return true;
    }
    private bool InitializeSpawnLeft(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x - 1, PlayerSpawn.y))
            return false;

        destructibleCoords.Remove(PlayerSpawn.x - 1, PlayerSpawn.y);
        destructibleCoords.Remove(PlayerSpawn.x - 2, PlayerSpawn.y);

        return true;
    }
    private bool InitializeSpawnRight(Coor PlayerSpawn)
    {
        if (indestructibleCoords.inList(PlayerSpawn.x + 1, PlayerSpawn.y))
            return false;

        destructibleCoords.Remove(PlayerSpawn.x + 1, PlayerSpawn.y);
        destructibleCoords.Remove(PlayerSpawn.x + 2, PlayerSpawn.y);

        return true;
    }

    private void InitializeUpgrades()
    {
        // TODO
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

    public void AddBomb(GameObject bomb, int x, int y, BombParams p)
	{
		bombs.Add (new BombCoords (bomb, x, y, p));
	}
	private BombParams RemoveBomb(int x, int y)
	{
		foreach (BombCoords bomb in bombs)
			if (bomb.x == x && bomb.y == y) {
                returnBombToPlayer(bomb.bomb);
                Destroy (bomb.bomb); 	// Destroy bomb game oject
				bombs.Remove (bomb);
				return bomb.p;
			}
        return new BombParams();
	}
	public bool OnBomb(int x, int y)
	{
		foreach (BombCoords bomb in bombs)
			if (bomb.x == x && bomb.y == y)
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
    private void returnBombToPlayer(GameObject bomb)
    {
        try
        {
            bomb.GetComponent<BombController>().parentPlayer.GetComponent<PlayerController>().numBombs++;
        }
        catch (MissingReferenceException e) // If the player dies before the bomb explodes, then we do not need to give them another one
        { }
    }

    public void LineBomb(int x, int y, string v, int numBombs)
    {
        //TODO Implement this
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
                destructibleCoords.Remove(x, y);
                break;
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
                destructibleCoords.Remove(x, y);
                break;
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
                destructibleCoords.Remove(x, y);
                break;
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
                destructibleCoords.Remove(x, y);
                break;
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
