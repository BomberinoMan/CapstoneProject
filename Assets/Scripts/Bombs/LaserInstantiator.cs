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

    private Vector2 _up = new Vector2(0.0f, 1.0f);
    private Vector2 _down = new Vector2(0.0f, -1.0f);
    private Vector2 _left = new Vector2(-1.0f, 0.0f);
    private Vector2 _right = new Vector2(1.0f, 0.0f);

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

        InstantiateInDirection(location, _up, paramaters);
        InstantiateInDirection(location, _down, paramaters);
        InstantiateInDirection(location, _left, paramaters);
        InstantiateInDirection(location, _right, paramaters);

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
                laser = Instantiate(GetLaser(direction), new Vector3(location.x + direction.x * i, location.y + direction.y * i, 0.0f), Quaternion.identity) as GameObject;
            else
                laser = Instantiate(GetMiddleLaser(direction), new Vector3(location.x + direction.x * i, location.y + direction.y * i, 0.0f), Quaternion.identity) as GameObject;

            //TODO refactor all of this to make it cleaner/faster
            laser.GetComponent<LaserController>().creationTime = Time.time;
            laser.GetComponent<LaserController>().paramaters = paramaters;
        }
    }

    private GameObject GetMiddleLaser(Vector2 direction)
    {
        if (direction == _up || direction == _down)
            return laserVert;
        else
            return laserHor;
    }

    private GameObject GetLaser(Vector2 direction)
    {
        if (direction == _up)
            return laserUp;
        else if (direction == _down)
            return laserDown;
        else if (direction == _left)
            return laserLeft;
        else
            return laserRight;
    }
}
