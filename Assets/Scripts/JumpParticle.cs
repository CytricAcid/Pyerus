﻿using UnityEngine;
using System.Collections;
using AC;

public class JumpParticle : MonoBehaviour {
	private Player Player;
	public ParticleSystem newParticle;
	

	// Use this for initialization
	void Start () {
		Player = KickStarter.player;
	}	

	void OnTriggerEnter (Collider other)
	{
		//newParticle = (ParticleSystem)Instantiate (ashParticle, Player.transform.position, transform.rotation);
		Instantiate (Resources.Load("AshPoof"), Player.transform.position, transform.rotation);
	}
	// Update is called once per frame
	void Update () {

	}
}
