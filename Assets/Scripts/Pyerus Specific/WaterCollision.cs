//using UnityEngine;
//using System.Collections;
//
//public class WaterCollision : MonoBehaviour {
//
//	private Player Pyerus;
//
//	void Start ()
//	{
//		//currentHealth = maxHealth;	
//		Pyerus = KickStarter.player;
//	}
//
//	void OnTriggerEnter (Collider other)
//	{
//		if (other.tag == "Water") {
//			Pyerus.GetComponent <Status> ().inWater = true;
//			Pyerus.GetComponent <Status> ().StartDamage ();
//		}
//	}
//	void OnTriggerExit (Collider other)
//	{
//		if (other.tag == "Water") {
//			Pyerus.GetComponent <Status> ().inWater = false;
//			Pyerus.GetComponent <Status> ().StartRegen ();
//		}
//	}
//}	