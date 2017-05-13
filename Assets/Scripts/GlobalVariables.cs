using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour{
	public static GlobalVariables Instance { get; private set; }
	public static int spawnPointIndexLoad = -1;

	public List<bool> TestLevelMoths;
	public List<bool> TownMoths;

	void Awake ()
	{
		Instance = this;
	}

//	public void UpdateList (bool k, int index)
//	{
//		TownMoths.Insert (index, k);
//	}
}
