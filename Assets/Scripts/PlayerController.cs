using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public GameObject Bomb;

    private Rigidbody2D rb;
    private Transform transform;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
    }

	void Update ()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        lastPosition = rb.position;

        if (ver == 0.0f && hor != 0.0f)
            rb.position = new Vector3(
                rb.position.x + Input.GetAxisRaw("Horizontal") * Speed,
                AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.y),
                0.0f);
        else if(hor == 0.0f && ver != 0.0f)
            rb.position = new Vector3(
                AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.x),
                rb.position.y + Input.GetAxisRaw("Vertical") * Speed,
                0.0f);

        if(Input.GetKeyDown("space"))
        {
            GameObject bomb = Instantiate(Bomb, new Vector3(AxisRounder.Round(0.49f, 0.51f, transform.position.x), AxisRounder.Round(0.49f, 0.51f, transform.position.y), 0.0f), Quaternion.identity) as GameObject;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PowerUp")
            HandlePowerUpCollision(other);
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
