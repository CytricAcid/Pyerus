using UnityEngine;
using System.Collections;
using AC;

public class Status : MonoBehaviour {

	private Player Pyerus;
	public int currentHealth;
	public int maxHealth;
	public int displayHealth;
	public bool inWater;
	public float waterDamageDelay;

	void Start ()
	{
		currentHealth = maxHealth;	
		Pyerus = KickStarter.player;
		StartCoroutine (HealthRegen ());

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Water") {
			inWater = true;
			StartCoroutine (WaterDamage ());
		}
	}
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Water") {
			inWater = false;
			StartCoroutine (HealthRegen ());
		}
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
			yield return new WaitForSeconds (1);	
			if (maxHealth > currentHealth) {	
				currentHealth++;
				if (currentHealth > maxHealth) {
					currentHealth = maxHealth;
				}
			}
		}
	}
}
