using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingWater : MonoBehaviour {
	GameObject Pyerus;
	public float force;

	void Awake ()
	{
		if ((Pyerus == null) && (GameObject.FindGameObjectWithTag("Player") != null)) 
		{
			Pyerus = GameObject.FindGameObjectWithTag("Player");
		} else {
			Debug.LogWarning ("WHERE IS PYERUS. SHE IS NOT HERE. PLS FIX");
		}
	}
	// Use this for initialization
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			Pyerus.GetComponent<PlayerMachine> ().addedForce = Vector3.up * force;
			Pyerus.GetComponent<PlayerMachine> ().AddVelocity();
		}

	}
}
