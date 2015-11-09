using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    public GameObject background;
    public GameObject indestructible;
    public GameObject destructible;
    public GameObject player;
    public int rows;
    public int columns;

    private Transform boardHolder;

    void InitializeBoard()
    {
        for (int x = -1; x <= columns; x++)
            for (int y = -1; y <= rows; y++)
            {
                // Background
                GameObject instance;

                if(x == -1 || y == -1 || x == columns || y == rows) //Border
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
                else if(x % 2 == 1 && y % 2 == 1) // Rows and columns
                    instance = Instantiate(indestructible, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
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
}
