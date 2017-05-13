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
			//Moth.isCollected = GlobalVariables.Instance.TownMoths [b];
			
			if (Moth.isCollected == false && Moth.spawnOnLevelStart == true) {
				Moth.Moth.SetActive (true);
			} else { // atm it considers moths that don't spawn on level start t obe collected. fix dat
				Moth.Moth.SetActive (false);
				mothsCurrentlyCollected++;
			}
		}
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
		//AddToCollection ();
	}
}
