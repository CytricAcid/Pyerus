using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	GameObject Pyerus;
	public GameObject[] SpawnPoints;
	public int spawnIndex;
	// Use this for initialization
	void Start () {
		if ((Pyerus == null) && (GameObject.FindGameObjectWithTag("Player") != null)) 
		{
			Pyerus = GameObject.FindGameObjectWithTag("Player");
		} else {
			Debug.LogWarning ("WHERE IS PYERUS. SHE IS NOT HERE. PLS FIX");
		}
		MovePyerus ();
	}
	
	// Update is called once per frame
	void MovePyerus () {
		if ((0 <= spawnIndex) && (spawnIndex <= SpawnPoints.Length) && (SpawnPoints [spawnIndex] != null)) {
			Pyerus.transform.position = SpawnPoints [spawnIndex].transform.position;
			Pyerus.transform.rotation = SpawnPoints [spawnIndex].transform.rotation;
		} else {
			Debug.LogError ("Spawn is out of range or does not exist!");
		}
	}

}
