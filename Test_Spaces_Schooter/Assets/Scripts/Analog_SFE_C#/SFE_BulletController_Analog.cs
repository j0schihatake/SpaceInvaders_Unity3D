using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFE_BulletController_Analog : MonoBehaviour
{

	public Enemy enemy = null;
	public float impulseForce = 10f;
	public GameObject muzzleFire = null;
	public GameObject AnchoredJoint2D = null;
	public GameObject explosion = null;
	public int damage = 0;
	public GameObject[] detachOnDeath;

	void Start ()
	{
		if (muzzleFire)
			Instantiate (muzzleFire, transform.position, transform.rotation);
		GetComponent<Rigidbody> ().AddForce (transform.forward * impulseForce, ForceMode.Impulse);

	}

	void OnCollisionEnter (Collision collision)
	{
		Main.Instance.missile_explosion.transform.position = this.gameObject.transform.position;
		Main.Instance.missile_Particle_System.Play ();
		//Instantiate (explosion, transform.position, transform.rotation);

		if (collision.gameObject.GetComponent<Enemy> ()) {
			collision.gameObject.GetComponent<Enemy> ().destroy_and_add_score (damage);
		}

		if (collision.gameObject.GetComponent<Player> ()) {
			collision.gameObject.GetComponent<Player> ().destroy_and_end_level ();
		}


		if (detachOnDeath.Length > 0) {
			for (var i = 0; i < detachOnDeath.Length; i++) {
				detachOnDeath [i].transform.parent = null;
				ParticleSystem PS;  
				PS = detachOnDeath [i].GetComponent <ParticleSystem> ();
				PS.enableEmission = false;
				Destroy (detachOnDeath [i], 5);
			}
		}
		Destroy (gameObject);
	}
}
