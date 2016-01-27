using System.Collections.Generic;
using UnityEngine;

public struct Coor
{
    public GameObject gameObject;
    public int x;
    public int y;

    public Coor(int x, int y, GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.x = x;
        this.y = y;
    }
}

public class Coords
{
    private List<Coor> coords = new List<Coor>();

    public void Add(int x, int y, GameObject gameObject)
    {
        coords.Add(new Coor(x, y, gameObject));
    }

    public GameObject Remove(int x, int y)
    {
        Coor itemToRemove = new Coor();

        foreach (Coor coor in coords)
            if (coor.x == x && coor.y == y)
                itemToRemove = coor;

        coords.Remove(itemToRemove);
        return itemToRemove.gameObject;
    }

    public GameObject Remove(Coor givenCoords)
    {
        Coor itemToRemove = new Coor();

        foreach (Coor coor in coords)
            if (coor.x == givenCoords.x && coor.y == givenCoords.y)
            {
                itemToRemove = coor;
            }

        coords.Remove(itemToRemove);
        return itemToRemove.gameObject;
    }

    public bool inList(int x, int y)
    {
        foreach (Coor coor in coords)
            if (coor.x == x && coor.y == y)
                return true;
        return false;
    }

    public bool inList(Coor givenCoords)
    {
        foreach (Coor coor in coords)
            if (coor.x == givenCoords.x && coor.y == givenCoords.y)
                return true;
        return false;
    }
}
