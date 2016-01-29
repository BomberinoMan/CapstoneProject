using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;

    void Awake()
    {
        boardManager = GetComponent<BoardManager>();
        //boardManager.CreateBoard();
    }
}
