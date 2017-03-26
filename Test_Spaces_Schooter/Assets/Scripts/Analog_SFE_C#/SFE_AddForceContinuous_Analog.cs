using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFE_AddForceContinuous_Analog : MonoBehaviour
{
	
	public float force = 10f;

	void Update ()
	{
		GetComponent<Rigidbody> ().AddForce (transform.forward * force * Time.deltaTime, ForceMode.Impulse);
	}
}
