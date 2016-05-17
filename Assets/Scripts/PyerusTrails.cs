using UnityEngine;
using System.Collections;
using AC;

public class PyerusTrails : MonoBehaviour {

	private Player Pyerus;
	private TrailRenderer trail;


	// Use this for initialization
	void Start () {
		Pyerus = KickStarter.player;
		trail = GetComponent<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Pyerus.GetComponent<Player> ().isGliding == true && Pyerus.GetComponent<Player> ().GetMoveSpeed() > 0f)
		{
			trail.enabled = true;
		}
		else
		{
			trail.enabled = false;
		}
	}
}
