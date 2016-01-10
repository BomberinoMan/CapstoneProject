using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float speedScalar;
    public int bombKick;
    public int bombLine;
    public int currNumBombs;

    private int _maxNumBombs;
    public int maxNumBombs
    {
        get { return _maxNumBombs; }
        set
        {
            Debug.Log(value);
            _maxNumBombs = value;
            currNumBombs++;
        }
    }
    public BombParams bombParams = new BombParams();
    public bool canLayBombs = true;
    public bool alwaysLayBombs = false;
    public bool reverseMovement = false;

    public GameObject bombObject;
    private Rigidbody2D rb;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Transform transform;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        _maxNumBombs = currNumBombs;
    }

	void Update ()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if(!reverseMovement)
        {
            if (ver == 0.0f && hor != 0.0f)
                rb.position = new Vector3(
                    rb.position.x + hor * speed * speedScalar,
                    AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.y),
                    0.0f);
            else if (hor == 0.0f && ver != 0.0f)
                rb.position = new Vector3(
                    AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.x),
                    rb.position.y + ver * speed * speedScalar,
                    0.0f);
        }
        else
        {
            if (ver == 0.0f && hor != 0.0f)
                rb.position = new Vector3(
                    rb.position.x - hor * speed * speedScalar,
                    AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.y),
                    0.0f);
            else if (hor == 0.0f && ver != 0.0f)
                rb.position = new Vector3(
                    AxisRounder.SmoothRound(0.3f, 0.7f, rb.position.x),
                    rb.position.y - ver * speed * speedScalar,
                    0.0f);
        }

        if ((Input.GetKeyDown("space") && canLayBombs) || alwaysLayBombs)
        {
			if (bombLine > 0 && GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().OnBomb((int)AxisRounder.Round(0.49f, 0.51f, transform.position.x), (int)AxisRounder.Round(0.49f, 0.51f, transform.position.y)))
				GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().LineBomb((int)AxisRounder.Round(0.49f, 0.51f, transform.position.x), (int)AxisRounder.Round(0.49f, 0.51f, transform.position.y), gameObject.GetComponentInChildren<PlayerAnimationDriver>().GetDirection(), currNumBombs, gameObject);
            else
            {
                if (currNumBombs <= 0)
                    return;

                GameObject bomb = Instantiate(
	                bombObject,
	                new Vector3(
	                    AxisRounder.Round(0.49f, 0.51f, transform.position.x),
	                    AxisRounder.Round(0.49f, 0.51f, transform.position.y),
	                    0.0f),
	                Quaternion.identity)
	                as GameObject;
				BombManager.SetupBomb(gameObject, bomb);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Upgrade")
        {
            UpgradeFactory.getUpgrade(other.gameObject.GetComponent<UpgradeType>().type).ApplyEffect(gameObject);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Laser")
        {
            //TODO add destruction animation support
            Destroy(gameObject);
        }
    }
}
