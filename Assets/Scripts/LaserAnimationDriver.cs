using UnityEngine;
using System.Collections;

public class LaserAnimationDriver : MonoBehaviour 
{
    public BombParams paramaters;
	private Animator animator;
	private float CreationTime;
	
	
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		CreationTime = Time.time;
	}
	
	void Update () {
		animator.SetBool("IsDone", (CreationTime + paramaters.explodingDuration) <= Time.time);
	}
	
	public void KillMe()
	{
		Destroy(gameObject.transform.parent.gameObject);
	}
}
