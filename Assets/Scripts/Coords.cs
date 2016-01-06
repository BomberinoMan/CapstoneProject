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

public class Coords {
    private List<Coor> coords = new List<Coor>();

    public void Add(int x, int y, GameObject gameObject)
    {
        coords.Add(new Coor(x, y, gameObject));
    }

    public void Remove(int x, int y)
    {
        Coor itemToRemove = new Coor();
        bool remove = false;

        foreach (Coor coor in coords)
            if (coor.x == x && coor.y == y)
            {
                itemToRemove = coor;
                remove = true;
            }

        if (remove)
            RemoveItem(itemToRemove);
    }

    public void Remove(Coor givenCoords)
    {
        Coor itemToRemove = new Coor();
        bool remove = false;

        foreach (Coor coor in coords)
            if (coor.x == givenCoords.x && coor.y == givenCoords.y)
            {
                itemToRemove = coor;
                remove = true;
            }

        if (remove)
            RemoveItem(itemToRemove);
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

    private void RemoveItem(Coor coor)
    {
        coords.Remove(coor);
        Object.Destroy(coor.gameObject.gameObject);
    }
}
   