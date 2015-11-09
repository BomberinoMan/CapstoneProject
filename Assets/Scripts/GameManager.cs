using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;

    void Awake()
    {
        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardManager.CreateBoard();
    }
}
