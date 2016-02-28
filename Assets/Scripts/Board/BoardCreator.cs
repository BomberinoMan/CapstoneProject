using UnityEngine;
using System.Collections;

public class BoardCreator
{
    private BoardParams boardParams;
    private Board board;

    public BoardCreator(int i = 0)
    {
        boardParams = BoardParamsFactory.getBoardParams(i);
        board = new Board();
    }

    public Board getBoard()
    {
        return board;
    }

    public void InitializeUpgrades()
    {
        InitializeUpgrades(boardParams.minNumBombUpgrades, boardParams.maxNumBombUpgrades, UpgradeType.Bomb);
        InitializeUpgrades(boardParams.minNumKickUpgrades, boardParams.maxNumKickUpgrades, UpgradeType.Kick);
        InitializeUpgrades(boardParams.minNumLaserUpgrades, boardParams.maxNumLaserUpgrades, UpgradeType.Laser);
        InitializeUpgrades(boardParams.minNumLineUpgrades, boardParams.maxNumLineUpgrades, UpgradeType.Line);
        InitializeUpgrades(boardParams.minNumRadioactiveUpgrades, boardParams.maxNumRadioactiveUpgrades, UpgradeType.Radioactive);
    }

    private void InitializeUpgrades(int min, int max, UpgradeType upgradeType)
    {
        for (int i = 0; i < max; i++)
        {
            int x = (int)UnityEngine.Random.Range(0.0f, board.columns - 1);
            int y = (int)UnityEngine.Random.Range(0.0f, board.rows - 1);

            if (board.tiles[x, y].isDestructible && !board.tiles[x, y].isUpgrade)
            {
                board.tiles[x, y].upgradeType = upgradeType;
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

    public void InitializeDestructible()
    {
        for (int x = 0; x < board.columns; x++)
            for (int y = 0; y < board.rows; y++)
            {
                if (Random.value <= boardParams.fillPercentage && !board.tiles[x, y].isIndestructible)
                    board.tiles[x, y].isDestructible = true;
            }
    }

    public void InitializeSpawn(Vector3 playerSpawn)
    {
        int x = (int)playerSpawn.x;
        int y = (int)playerSpawn.y;
        board.tiles[x, y].isDestructible = false;
        board.tiles[x, y].isUpgrade = false;

        board.tiles[x, y].isDestructible = false;
        board.tiles[x, y].isUpgrade = false;

        if (!InitializeSpawnUp(x, y))
            if (!InitializeSpawnRight(x, y))
                InitializeSpawnLeft(x, y);

        if (!InitializeSpawnDown(x, y))
            if (!InitializeSpawnLeft(x, y))
                InitializeSpawnRight(x, y);
    }

    private bool InitializeSpawnUp(int x, int y)
    {
        if (board.tiles[x, y + 1].isIndestructible)
            return false;

        board.tiles[x, y + 1].isDestructible = false;
        board.tiles[x, y + 1].isUpgrade = false;

        board.tiles[x, y + 2].isDestructible = false;
        board.tiles[x, y + 2].isUpgrade = false;

        return true;
    }

    private bool InitializeSpawnDown(int x, int y)
    {
        if (board.tiles[x, y - 1].isIndestructible)
            return false;

        board.tiles[x, y - 1].isDestructible = false;
        board.tiles[x, y - 1].isUpgrade = false;

        board.tiles[x, y - 2].isDestructible = false;
        board.tiles[x, y - 2].isUpgrade = false;

        return true;
    }

    private bool InitializeSpawnLeft(int x, int y)
    {
        if (board.tiles[x - 1, y].isIndestructible)
            return false;

        board.tiles[x - 1, y].isDestructible = false;
        board.tiles[x - 1, y].isUpgrade = false;

        board.tiles[x - 2, y].isDestructible = false;
        board.tiles[x - 2, y].isUpgrade = false;

        return true;
    }

    private bool InitializeSpawnRight(int x, int y)
    {
        if (board.tiles[x + 1, y].isIndestructible)
            return false;

        board.tiles[x + 1, y].isDestructible = false;
        board.tiles[x + 1, y].isUpgrade = false;

        board.tiles[x + 2, y].isDestructible = false;
        board.tiles[x + 2, y].isUpgrade = false;

        return true;
    }
}
