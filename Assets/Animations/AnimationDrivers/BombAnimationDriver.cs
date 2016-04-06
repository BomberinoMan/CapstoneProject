using UnityEngine;

public class BombAnimationDriver : MonoBehaviour
{
    private Animator animator;
    public BombParams paramaters;
    private float _creationTime;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        _creationTime = Time.time;
        paramaters = gameObject.GetComponentInParent<BombController>().paramaters;
    }

    void FixedUpdate()
    {
        animator.SetBool("IsExploding", (_creationTime + paramaters.delayTime) <= Time.time);
        //animator.SetBool("IsDisapearing", (_creationTime + paramaters.delayTime + paramaters.warningTime + paramaters.explodingDuration) <= Time.time);
    }
}
