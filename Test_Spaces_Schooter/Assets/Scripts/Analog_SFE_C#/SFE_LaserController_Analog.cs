using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFE_LaserController_Analog : MonoBehaviour
{
	public BoxCollider boxCollider;
	public float damage = 3f;

	public GameObject impactEffect;
	public GameObject muzzleEffect;


	void Start ()
	{	
		Destroy (boxCollider, 0.08f);  //don't ask.... just to make extra sure the box collider doesn't stay out too long, even if... eh it is complicated, this is needed.
		if (muzzleEffect)
			Instantiate (muzzleEffect, transform.position, transform.rotation);

		Vector3 direction = transform.position;
		RaycastHit hit = new RaycastHit ();

		if (Physics.Raycast (transform.position, direction, hit.distance)) { // if it hits something, this happens
			boxCollider.size.Set (boxCollider.size.x, (float)hit.distance, boxCollider.size.z);  //I set up a box collider to be along the laser...
			float y_k = boxCollider.center.y + (hit.distance / 2);
			boxCollider.center.Set (boxCollider.center.x, y_k, boxCollider.center.z);
			if (impactEffect)
				Instantiate (impactEffect, hit.point, hit.transform.rotation);
		} else {   //if the raycast hits nothing
			Destroy (boxCollider); //if the laser didn't hit anything, then we have to get rid of the box collider, else it just stays there...
		}
	}

	void OnCollisionEnter (Collision collision)
	{ //we just want to hit it once, so after a collision we destroy the collider...
		Destroy (boxCollider);
	}
}
