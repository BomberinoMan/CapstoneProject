using UnityEngine;
using System.Collections;

public class BombAnimationDriver : MonoBehaviour {
    private Animator animator;
    public BombParams paramaters;
    private float CreationTime;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
        CreationTime = Time.time;
        paramaters = gameObject.GetComponentInParent<BombController>().paramaters;
    }

    void FixedUpdate () {
        animator.SetBool("IsExploding", (CreationTime + paramaters.delayTime) <= Time.time);
        animator.SetBool("IsDisapearing", (CreationTime + paramaters.delayTime + paramaters.explodingDuration) <= Time.time);
    }

    void KillMe()
    {
		gameObject.transform.parent.GetComponent<BombController>().Explode();
    }
}
