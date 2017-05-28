using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothScript : MonoBehaviour {
	GeneralUI GameManager;
	void Start ()
	{
		if ((GameManager == null) && (GameObject.Find ("GameManager") != null)) {
			GameManager = GameObject.Find ("GameManager").GetComponent<GeneralUI> ();
		} else {
			Debug.LogWarning ("There is no Game Manager in the scene!");
		}
	}
	void OnTriggerEnter (Collider other){
		if (other.tag == "Player") {
			GameManager.MothCollection (gameObject);
		}
	}
}
