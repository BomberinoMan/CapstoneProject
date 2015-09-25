using UnityEngine;
using System.Collections;

public class DesctroyByBoundary : MonoBehaviour 
{	
	void OnTriggerExit(Collider other)
	{
		Destroy (other.gameObject);
	}
}
