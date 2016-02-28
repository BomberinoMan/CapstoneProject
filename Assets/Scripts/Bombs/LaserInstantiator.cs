using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class LaserInstantiator : NetworkBehaviour
{
    public GameObject laserCross;
    public GameObject laserUp;
    public GameObject laserDown;
    public GameObject laserLeft;
    public GameObject laserRight;
    public GameObject laserHor;
    public GameObject laserVert;

    private Vector2 up = new Vector2(0.0f, 1.0f);
    private Vector2 down = new Vector2(0.0f, -1.0f);
    private Vector2 left = new Vector2(-1.0f, 0.0f);
    private Vector2 right = new Vector2(1.0f, 0.0f);

    public void InstantiateLaser()
    {
        var temp = gameObject.GetComponent<BombController>().paramaters;
        var paramaters = new BombParams
        {
            radius = temp.radius,
            delayTime = temp.delayTime,
            warningTime = temp.warningTime,
            explodingDuration = temp.explodingDuration
        };
        var location = new Vector2(AxisRounder.Round(gameObject.transform.position.x), AxisRounder.Round(gameObject.transform.position.y));

        GameObject laser = Instantiate(laserCross, location, Quaternion.identity) as GameObject;
        //TODO refactor this too
        laser.GetComponent<LaserController>().creationTime = Time.time;
        laser.GetComponent<LaserController>().paramaters = paramaters;
        NetworkServer.Spawn(laser);

        laser.GetComponent<LaserController>().RpcSetupLaser(Time.time, paramaters);

        InstantiateInDirection(location, up, paramaters);
        InstantiateInDirection(location, down, paramaters);
        InstantiateInDirection(location, left, paramaters);
        InstantiateInDirection(location, right, paramaters);

        NetworkServer.Destroy(gameObject);
    }



    private void InstantiateInDirection(Vector2 location, Vector2 direction, BombParams paramaters)
    {
        var emptySpace = Physics2D.RaycastAll(location, direction)
            .Where(h => h.distance != 0 && h.transform.tag != "Laser" && h.transform.tag != "Bomb" && h.transform.tag != "Player")
            .First();

        int numLasers = emptySpace.distance < paramaters.radius ? (int)emptySpace.distance : paramaters.radius;

        if ((emptySpace.transform.tag == "Destructible" || emptySpace.transform.tag == "Upgrade") && emptySpace.distance <= paramaters.radius)
            NetworkServer.Destroy(emptySpace.transform.gameObject);

        for (int i = 1; i <= numLasers; i++)
        {
            GameObject laser;
            if (i == paramaters.radius)
                laser = Instantiate(getLaser(direction), new Vector3(location.x + direction.x * i, location.y + direction.y * i, 0.0f), Quaternion.identity) as GameObject;
            else
                laser = Instantiate(getMiddleLaser(direction), new Vector3(location.x + direction.x * i, location.y + direction.y * i, 0.0f), Quaternion.identity) as GameObject;

            //TODO refactor all of this to make it cleaner/faster
            laser.GetComponent<LaserController>().creationTime = Time.time;
            laser.GetComponent<LaserController>().paramaters = paramaters;
            NetworkServer.Spawn(laser);
            laser.GetComponent<LaserController>().RpcSetupLaser(Time.time, paramaters);
        }
    }

    private GameObject getMiddleLaser(Vector2 direction)
    {
        if (direction == up || direction == down)
            return laserVert;
        else
            return laserHor;
    }

    private GameObject getLaser(Vector2 direction)
    {
        if (direction == up)
            return laserUp;
        else if (direction == down)
            return laserDown;
        else if (direction == left)
            return laserLeft;
        else
            return laserRight;
    }
}
