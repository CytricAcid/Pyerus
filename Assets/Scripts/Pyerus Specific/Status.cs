using UnityEngine;
using System.Collections;
using AC;

public class Status : MonoBehaviour {

	private Player Pyerus;

	void Start ()
	{
		//Pyerus = GameObject.FindObjectOfType <Player> ();
		Pyerus = KickStarter.player;
	}
	// Use this for initialization
}
