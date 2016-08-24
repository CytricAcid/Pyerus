using UnityEngine;
using System.Collections;
using AC;

public class JumpParticle : MonoBehaviour {
	private Player Player;
	public GameObject particle;
	

	// Use this for initialization
	void Start () {
		Player = KickStarter.player;
	}	

	void OnTriggerEnter (Collider other)
	{
		if (Player.IsGroundedParticle() == false) {
			Instantiate (particle, Player.transform.position, transform.rotation);
		}
	}
	// Update is called once per frame
	void Update () {

	}
}
