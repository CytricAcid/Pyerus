using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RockJumpScript : MonoBehaviour {

	GeneralUI GameManager;
	void Start ()
	{
		if ((GameManager == null) && (GameObject.Find ("GameManager") != null)) {
			GameManager = GameObject.Find ("GameManager").GetComponent<GeneralUI> ();
		} else {
			print ("There is no Game Manager in the scene!");
		}
	}

	[System.Serializable]
	public class RocksArray
	{
		public GameObject Rock;
		public bool jumpedOn = false;

	}
	public Material JumpedOnMaterial;
	public bool mothSpawned;

	public RocksArray[] ListOfRocks;


	private bool IsAllRocksJumpedOn() {
		for (int i = 0; i < ListOfRocks.Length; i++) {
			if (ListOfRocks[i].jumpedOn == false){
				
				return false;
			}
		}

		return true;
	}

	public void ColorChange (GameObject other)
	{
		int rockIndex = System.Array.IndexOf(ListOfRocks, other);
		print (rockIndex);
		//ListOfRocks [rockIndex].jumpedOn = true;
		other.GetComponent<MeshRenderer>().material = JumpedOnMaterial;
	}

	void SpawnMoth () {
		print ("this is a debug message to tell you that your moth has been delivered");
		mothSpawned = true;
	}
	
	// Update is called once per frame
	void JumpRockDone () {
		if (IsAllRocksJumpedOn() && mothSpawned == false) {
			SpawnMoth ();
			print ("heck yeah u done did it friend");
		}
	}			

}