using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFE_destroyThisTimed_Analog : MonoBehaviour
{

	public float destroyTime = 1f;

	void Start ()
	{
		Destroy (gameObject, destroyTime);
	}
}
