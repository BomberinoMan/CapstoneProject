using UnityEngine;
using System.Collections.Generic;

struct BombCoords{
	public GameObject bomb;
	public int x;
	public int y;
	public int r;

	public BombCoords(GameObject bomb, int x, int y, int r)
	{
		this.bomb = bomb;
		this.x = x;
		this.y = y;
		this.r = r;
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

	public void AddBomb(GameObject bomb, int x, int y, int r)
	{
		bombs.Add (new BombCoords (bomb, x, y, r));
	}

	private int RemoveBomb(int x, int y)
	{
		foreach (BombCoords bomb in bombs)
			if (bomb.x == x && bomb.y == y) {
				Destroy (bomb.bomb); 	// Destroy bomb game oject
				bombs.Remove (bomb);
				return bomb.r;
			}
		return -1;
	}

	private bool OnBomb(int x, int y)
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
		int r = RemoveBomb ((int)x, (int)y);	// Remove from list of bombs, destroy GameObject
		GameObject instance = Instantiate (laserCross, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent (boardHolder);	// Instantiate the cross of the laser

		LaserUp (x, y, r);
		LaserDown (x, y, r);
		LaserLeft (x, y, r);
		LaserRight (x, y, r);
	}

	private void LaserUp(int x, int y, int radius)
	{
		GameObject laser;
		for (int i = 0; i <= radius; i++) {
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
			if(i != radius)
				laser = Instantiate(laserVert, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserUp, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;

			laser.transform.SetParent(boardHolder);
		}
	}

	private void LaserDown(int x, int y, int radius)
	{
		GameObject laser;
		for (int i = 0; i <= radius; i++) {
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
			if(i != radius)
				laser = Instantiate(laserVert, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserDown, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			
			laser.transform.SetParent(boardHolder);
		}
	}

	private void LaserLeft(int x, int y, int radius)
	{
		GameObject laser;
		for (int i = 0; i <= radius; i++) {
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
			if(i != radius)
				laser = Instantiate(laserHor, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserLeft, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			
			laser.transform.SetParent(boardHolder);
		}
	}

	private void LaserRight(int x, int y, int radius)
	{
		GameObject laser;
		for (int i = 0; i <= radius; i++) {
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
			if(i != radius)
				laser = Instantiate(laserHor, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			else 
				laser = Instantiate(laserRight, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
			
			laser.transform.SetParent(boardHolder);
		}
	}
}
