using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AC;

public class PlayerScript : MonoBehaviour {

	public GameObject Pyerus;
	public bool isGround;

	public Text groundedText;
	
	// Update is called once per frame
	void Update () 
	{
		if (Pyerus.GetComponent<Player> ().IsGrounded ()) {
			isGround = true;
		} else {
			isGround = false;
		}
		groundedText.text = "On Ground:" + isGround;

	}
}
