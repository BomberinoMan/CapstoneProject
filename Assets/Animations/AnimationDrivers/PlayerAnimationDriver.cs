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
    private DPadController dPad;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
        isLocalPlayer = gameObject.GetComponentInParent<PlayerControllerComponent>().isLocalPlayer;
        dPad = gameObject.GetComponentInParent<PlayerControllerComponent>().dPad;
    }

    void FixedUpdate () {
        if (!isLocalPlayer)
            return;
        //TODO need to implement reverse movement here aswell
        //TODO refactor
        float hor = dPad.currDirection.x;
        float vert = dPad.currDirection.y;

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

    public Vector2 GetDirection()
    {
        switch (animator.GetInteger("Direction"))
        {
			case 1: 
				return new Vector2(0.0f, 1.0f); // Up
			case 2: 
				return new Vector2(1.0f, 0.0f); // Right
			case 3: 
				return new Vector2(0.0f, -1.0f); // Down
			case 4: 
				return new Vector2(-1.0f, 0.0f); // Left
			default: 
 				return new Vector2(1.0f, 0.0f); // Right
        }
    }
}
