using UnityEngine;
using System.Collections;

public class LaserAnimationDriver : MonoBehaviour 
{
	
	private Animator animator;
	private float CreationTime;
	private float ExplosionTime;
	
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		CreationTime = Time.time;
		ExplosionTime = BombParams.ExplodingDuration;
	}
	
	void Update () {
		//animator.SetBool("IsDone", (CreationTime + ExplosionTime) <= Time.time);
	}
	
	public void KillMe()
	{
		Destroy(gameObject.transform.parent.gameObject);
	}
}
