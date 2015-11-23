using UnityEngine;
using System.Collections;

public class BombAnimationDriver : MonoBehaviour {
    private float DelayTime;
    private float ExplosionTime;
    private Animator animator;
    private float CreationTime;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
        CreationTime = Time.time;

		DelayTime = BombParams.DelayTime;
		ExplosionTime = BombParams.WarningTime;
    }

    void Update () {
        animator.SetBool("IsExploding", (CreationTime + DelayTime) <= Time.time);
        animator.SetBool("IsDisapearing", (CreationTime + DelayTime + ExplosionTime) <= Time.time);
    }

    void KillMe()
    {
		gameObject.transform.parent.GetComponent<BombController>().Explode();
    }
}
