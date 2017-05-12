using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothScript : MonoBehaviour {
	public bool isCollected;
	public bool spawnOnLevelStart;
	// Use this for initialization
	void OnTriggerEnter (Collider other){
		if (other.tag == "Player") {
			print ("hi");
			gameObject.SetActive (false);
		}
	}
}
