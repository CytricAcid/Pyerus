using UnityEngine;
using System.Collections;
using AC;

public class KillPlane : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter (Collider other) {
		KickStarter.TurnOffAC ();
		Destroy (other.gameObject,3f);
	}
}
