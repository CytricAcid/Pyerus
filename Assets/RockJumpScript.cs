using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RockJumpScript : MonoBehaviour {

	GeneralUI GameManager;
	int rockIndex;
	public GameObject MothReward;
	void Start ()
	{
		if ((GameManager == null) && (GameObject.Find ("GameManager") != null)) {
			GameManager = GameObject.Find ("GameManager").GetComponent<GeneralUI> ();
		} else {
			Debug.LogWarning ("There is no Game Manager in the scene!");
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

	public void JumpOnRock (GameObject other)
	{
		for (int i = 0; i < ListOfRocks.Length; i++) { //compares object that called this script to the array
			if (ListOfRocks [i].Rock == other) {
				rockIndex = i;
				print (rockIndex);
				break;
			} else {
				rockIndex = -1;
			}
		}
		if (rockIndex != -1 && !ListOfRocks [rockIndex].jumpedOn) { //if it matches the array, do this stuff
			ListOfRocks [rockIndex].jumpedOn = true;
			JumpRockDone ();
			other.GetComponent<MeshRenderer>().material = JumpedOnMaterial;
		} else {
			if (!ListOfRocks [rockIndex].jumpedOn) {
				Debug.LogWarning ("Rock does not exist in the array!");
			}
		}
	}

	void SpawnMoth () {
		if (MothReward != null) {
			GameManager.EnableMoth (MothReward);
			mothSpawned = true;
			print ("this is a debug message to tell you that your moth has been delivered");
		} else {
			Debug.LogWarning ("No Moth set as reward!");
		}
	}
	
	// Update is called once per frame
	void JumpRockDone () {
		if (IsAllRocksJumpedOn() && mothSpawned == false) {
			SpawnMoth ();
		}
	}			

}