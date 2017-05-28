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
		{
			int b = 0;
			
			if (Moth.isCollected == false && Moth.spawnOnLevelStart == true) {
				Moth.Moth.SetActive (true);
			}
			else {
				Moth.Moth.SetActive (false);
				if (Moth.isCollected == true) {
					mothsCurrentlyCollected++;
				}
			}
		}
		UpdateUI ();
	}

	public void EnableMoth (GameObject Moth) {
		if (Moth.CompareTag ("Moth")){
			for (int i = 0; i < ListOfMoths.Length; i++) { //Compares Moth collected with the list of Moths and enables it
				if (ListOfMoths [i].Moth == Moth) {
					ListOfMoths [i].Moth.SetActive (true);
					break;
				} 
			}
		}
	}


	public void MothCollection (GameObject Moth){
		for (int i = 0; i < ListOfMoths.Length; i++) { //Compares Moth collected with the list of Moths and marks the one collected as collected
			if (ListOfMoths [i].Moth == Moth) {
				ListOfMoths [i].isCollected = true;
				break;
			}
		}
		Moth.SetActive (false);
		mothsCurrentlyCollected++;
		UpdateUI ();
	}

	public void AddToCollection (){
		int i = 0;
		foreach (MothEntry Moth in ListOfMoths)
		{
			GlobalVariables.Instance.TownMoths.Insert (i,Moth.isCollected);
			i++;
		}
	}
		
	public void UpdateUI (){
		MothCount.text = mothsCurrentlyCollected + "/" + ListOfMoths.Length;
	}
}
