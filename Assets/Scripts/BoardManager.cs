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

struct Coords
{
	public int x;
	public int y;

	public Coords(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public class BoardManager : MonoBehaviour {

    public GameObject background;
    public GameObject indestructible;
    public GameObject destructible;
    public GameObject player;
	
	public GameObject laserCross;
	public GameObject laserHor;
	public GameObject laserVert;
	public GameObject laserUp;
	public GameObject laserDown;
	public GameObject laserLeft;
	public GameObject laserRight;

    public int rows;
    public int columns;

    private Transform boardHolder;
	private List<BombCoords> bombs = new List<BombCoords>();
	private List<Coords> indestructibleCoords = new List<Coords>();
	

    void InitializeBoard()
    {
        for (int x = -1; x <= columns; x++)
            for (int y = -1; y <= rows; y++)
            {
                // Background
                GameObject instance;

                if(x == -1 || y == -1 || x == columns || y == rows) //Border
				{
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
					indestructibleCoords.Add(new Coords(x, y));
				}
                else if(x % 2 == 1 && y % 2 == 1) // Rows and columns
				{
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
					indestructibleCoords.Add(new Coords(x, y));
				}
                else
                    instance = Instantiate(background, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
    }

    public void LineBomb(int x, int y, string v, int numBombs)
    {
        //TODO Implement this
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

    void InitializePlayers()
    {
        GameObject instance = Instantiate(player, new Vector3(0, rows-1, 0.0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(boardHolder);
    }

    public void CreateBoard()
    {
        boardHolder = new GameObject("Board").transform;
        InitializeBoard();
        InitializePlayers();
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

	private bool OnIndestructible(int x, int y)
	{
		foreach (Coords square in indestructibleCoords)
			if (square.x == x && square.y == y)
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
			else if(OnIndestructible(x, y))
			{
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
			else if(OnIndestructible(x, y))
			{
				break;
			}
			if(i != p.radius)
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
			else if(OnIndestructible(x, y))
			{
				break;
			}
			if(i != p.radius)
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
			else if(OnIndestructible(x, y))
			{
				break;
			}
			if(i != p.radius)
				laser = Instantiate(laserHor, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserRight, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
            laser.GetComponent<LaserController>().paramaters = p;

            laser.transform.SetParent(boardHolder);
		}
	}
}
