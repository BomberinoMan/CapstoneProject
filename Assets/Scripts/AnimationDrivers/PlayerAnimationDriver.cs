using UnityEngine;
using UnityEngine.Networking;

public static class Direction
{
    public static int Up = 1;
    public static int Right = 2;
    public static int Down = 3;
    public static int Left = 4;
}

public class PlayerAnimationDriver : NetworkBehaviour {
    [SyncVar]
    private Animator animator;
    private new bool isLocalPlayer;

	void Start () {
        animator = gameObject.GetComponent<Animator>();
        isLocalPlayer = gameObject.GetComponentInParent<PlayerControllerComponent>().isLocalPlayer;
    }
	
	void FixedUpdate () {
        if (!isLocalPlayer)
            return;
        //TODO need to implement reverse movement here aswell
        float vert = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");

        if (vert != 0.0) {
            animator.SetFloat("Speed", Mathf.Abs(vert));

            if (vert > 0.0)
                animator.SetInteger("Direction", Direction.Up);
            else if (vert < 0.0)
                animator.SetInteger("Direction", Direction.Down);
        }
        else if (hor != 0.0) {
            animator.SetFloat("Speed", Mathf.Abs(hor));

            if (hor > 0.0)
                animator.SetInteger("Direction", Direction.Right);
            else if (hor < 0.0)
                animator.SetInteger("Direction", Direction.Left);
        }
        else
            animator.SetFloat("Speed", 0.0f);
    }

    public string GetDirection()
    {
        switch (animator.GetInteger("Direction"))
        {
            case 1: return "Up";
            case 2: return "Right";
            case 3: return "Down";
            case 4: return "Left";
            default: return "Up";
        }
    }
}
