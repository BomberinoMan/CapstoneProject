public class Board
{
    public int columns;
    public int rows;
    public Tile[,] tiles;

    public Board(bool blank = false)
    {
        columns = 15;
        rows = 13;
        tiles = new Tile[columns, rows];

        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
            {
                tiles[x, y] = new Tile();
                tiles[x, y].x = (float)x;
                tiles[x, y].y = (float)y;

                // Assign indestructible
                if (x == 0 || y == 0 || x == columns - 1 || y == rows - 1) //Border
                    tiles[x, y].isIndestructible = true;
                else if (!blank && x % 2 == 0 && y % 2 == 0) // Rows and columns
                    tiles[x, y].isIndestructible = true;
            }
    }
}
