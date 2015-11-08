using UnityEngine;

public static class Direction
{
    public static int Up = 1;
    public static int Right = 2;
    public static int Down = 3;
    public static int Left = 4;
}

public class PlayerMovement : MonoBehaviour {
    public Animator animator;

	void Start () {
        animator = gameObject.GetComponent<Animator>();
	}
	
	void Update () {
        UpdateMovementAnimations();
    }

    void UpdateMovementAnimations()
    {
        float vert = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");

        if (vert != 0.0)
        {
            animator.SetFloat("Speed", Mathf.Abs(vert));

            if (vert > 0.0)
                animator.SetInteger("Direction", Direction.Up);
            else if (vert < 0.0)
                animator.SetInteger("Direction", Direction.Down);
        }
        else if (hor != 0.0)
        {
            animator.SetFloat("Speed", Mathf.Abs(hor));

            if (hor > 0.0)
                animator.SetInteger("Direction", Direction.Right);
            else if (hor < 0.0)
                animator.SetInteger("Direction", Direction.Left);
        }
        else
            animator.SetFloat("Speed", 0.0f);
    }
}
