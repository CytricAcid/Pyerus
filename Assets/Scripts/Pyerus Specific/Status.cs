using UnityEngine;
using System.Collections;
using AC;

public class Status : MonoBehaviour {

	private Player Pyerus;
	public int currentHealth;
	public int maxHealth;
	public bool inWater;
	public float waterDamageDelay;
	public float healthRegenDelay;
	private bool gameOver;

	void Start ()
	{
		gameOver = false;
		currentHealth = maxHealth;	
		Pyerus = KickStarter.player;
		StartCoroutine (HealthRegen ());
	}

	void Update ()
	{
		if (currentHealth < 1 && !gameOver) {
			KickStarter.TurnOffAC ();
			Destroy (Pyerus.gameObject, 3f);
			Application.LoadLevel (1);
			gameOver = true;
		}
	}


	public void StartDamage ()
	{
		StartCoroutine (WaterDamage ());
	}

	public void StartRegen ()
	{
		StartCoroutine (HealthRegen ());
	}

	IEnumerator WaterDamage ()
	{
		while (inWater == true) 
		{
			if (currentHealth > 0) {	
				currentHealth--;
			}
			yield return new WaitForSeconds (waterDamageDelay);
		}
	}

	IEnumerator HealthRegen ()
	{	
		while (inWater == false) 
		{
			yield return new WaitForSeconds (healthRegenDelay);	
			if (maxHealth > currentHealth) {	
				currentHealth++;
				if (currentHealth > maxHealth) {
					currentHealth = maxHealth;
				}
			}
		}
	}
}
