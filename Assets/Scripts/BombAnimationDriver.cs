using UnityEngine;
using System.Collections;

public class BombAnimationDriver : MonoBehaviour {
    public float DelayTime;
    public float ExplosionTime;

    private Animator animator;
    private float CreationTime;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
        CreationTime = Time.time;
    }

    void Update () {
        animator.SetBool("IsExploding", (CreationTime + DelayTime) <= Time.time);
        animator.SetBool("IsDisapearing", (CreationTime + DelayTime + ExplosionTime) <= Time.time);
    }

    public void KillMe()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
