using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float speedScalar;
    public int bombKick;
    public int bombLine;
    public int numBombs;
    public BombParams bombParams = new BombParams();
    public bool canLayBombs = true;
    public bool alwaysLayBombs = false;

    public GameObject bombObject;
    private Rigidbody2D rb;
    private Transform transform;

    void Start()
    {
        //TODO: Remove these once they are confirmed to be working
        Debug.LogWarning("Delay time: " + bombParams.delayTime);
        Debug.LogWarning("Exploding Duration: " + bombParams.explodingDuration);
        Debug.LogWarning("Radius: " + bombParams.radius);
        Debug.LogWarning("Warning Time: " + bombParams.warningTime);
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
    }

	void Update ()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (ver == 0.0f && hor != 0.0f)
            rb.position = new Vector3(
                rb.position.x + Input.GetAxisRaw("Horizontal") * speed,
                AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.y),
                0.0f);
        else if(hor == 0.0f && ver != 0.0f)
            rb.position = new Vector3(
                AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.x),
                rb.position.y + Input.GetAxisRaw("Vertical") * speed,
                0.0f);

        if((Input.GetKeyDown("space") && canLayBombs) || alwaysLayBombs)
        {
            if (GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().OnBomb((int)transform.position.x, (int)transform.position.y))
                GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().LineBomb((int)transform.position.x, (int)transform.position.y, gameObject.GetComponent<PlayerAnimationDriver>().GetDirection(), numBombs);
            else
            {
                if (numBombs <= 0)
                    return;
                numBombs--;

                GameObject bomb = Instantiate(
                bombObject,
                new Vector3(
                    AxisRounder.Round(0.49f, 0.51f, transform.position.x),
                    AxisRounder.Round(0.49f, 0.51f, transform.position.y),
                    0.0f),
                Quaternion.identity)
                as GameObject;
                bomb.GetComponent<BombController>().paramaters = bombParams;
                bomb.GetComponent<BombController>().parentPlayer = gameObject;
                bomb.transform.SetParent(gameObject.transform.parent);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PowerUp")
            HandlePowerUpCollision(other);
        else if (other.gameObject.tag == "Laser")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bomb")
            other.isTrigger = false;
    }

    private void HandlePowerUpCollision(Collider2D other)
    {
        // Do nothing
    }
}
