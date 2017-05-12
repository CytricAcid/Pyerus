using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUI : MonoBehaviour {
	public Text MothCount;
	public int mothsCurrentlyCollected;

	[System.Serializable]
	public class MothEntry
	{
		public GameObject Moth;
		public bool isCollected;
		public bool spawnOnLevelStart = true;
	}

	public MothEntry[] ListOfMoths;

	// Use this for initialization
	void Start () {
		foreach (MothEntry Moth in ListOfMoths)
		//for (int i = 0; i < MothSpawns.Length; i++)
		{
			if (Moth.isCollected == false && Moth.spawnOnLevelStart == true) {
				Moth.Moth.SetActive (true);
			} else {
				Moth.Moth.SetActive (false);
				mothsCurrentlyCollected++;
			}
		}
		UpdateUI ();
	}

	public void UpdateUI (){
		MothCount.text = mothsCurrentlyCollected + "/" + ListOfMoths.Length;
	}
}
