using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockJumpCollision : MonoBehaviour {

	public RockJumpScript RockJumpScript;


	// Use this for initialization


	void OnTriggerEnter () {
		print ("hi");
		RockJumpScript.JumpOnRock (gameObject);
	}
}
