using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
	public float speed { get; set; }
	public float speedScalar { get; set; }
	public int bombKick { get; set; }
	public int bombLine { get; set; }
	public int currNumBombs { get; set; }

    private int _maxNumBombs;
    public int maxNumBombs
    {
        get { return _maxNumBombs; }
        set
        {
            _maxNumBombs = value;
            currNumBombs++;
        }
    }
	public BombParams bombParams { get; set; }
	public bool canLayBombs { get; set; }
	public bool alwaysLayBombs { get; set; }
	public bool reverseMovement { get; set; }
	public bool isRadioactive { get; set; }

    public GameObject bombObject;
    private Rigidbody2D rb;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private Transform transform;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    void Start()
    {
		speed = 0.06f;
		speedScalar = 1.0f;
		bombKick = 0;
		bombLine = 0;
		currNumBombs = 1;
		_maxNumBombs = currNumBombs;
		bombParams = new BombParams();
		canLayBombs = true;
		alwaysLayBombs = false;
		reverseMovement = false;
		isRadioactive = false;

        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
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
			if (bombLine > 0 && GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>().OnBomb((int)AxisRounder.Round(transform.position.x), (int)AxisRounder.Round(transform.position.y)))
				GameObject.FindGameObjectWithTag("GameController")
					.GetComponent<BoardManager>()
						.LineBomb(
							(int)AxisRounder.Round(transform.position.x), 
							(int)AxisRounder.Round(transform.position.y), 
							gameObject.GetComponentInChildren<PlayerAnimationDriver>().GetDirection(), 
							currNumBombs, 
							gameObject);
            else
            {
                if (currNumBombs <= 0)
                    return;

                GameObject bomb = Instantiate(
	                bombObject,
	                new Vector3(
	                    AxisRounder.Round(transform.position.x),
	                    AxisRounder.Round(transform.position.y),
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
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardManager> ().Reassign ();
            //TODO add destruction animation support
            //Destroy(gameObject);
        }
    }
}
