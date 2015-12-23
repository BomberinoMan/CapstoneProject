using UnityEngine;

public class LaserController : MonoBehaviour {
    public BombParams paramaters;
    private float creationTime;

    void Start()
    {
        creationTime = Time.time;
    }

    void Update()
    {
        if (creationTime + paramaters.explodingDuration <= Time.time)
            Destroy(gameObject);
    }

}
