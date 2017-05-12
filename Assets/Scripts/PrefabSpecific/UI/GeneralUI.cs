using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUI : MonoBehaviour {
	public List<GameObject> MothSpawns;
	public List<bool> MothCollected;
	// Use this for initialization
	void Start () {
		foreach (GameObject Moth in MothSpawns)
		//for (int i = 0; i < MothSpawns.Length; i++)
		{
			if (Moth.GetComponent<MothScript> ().isCollected == false) {
				Moth.SetActive (true);
			} else {
				Moth.SetActive (false);
			}
		}
	}
}
