using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	GameObject Pyerus;
	public GameObject[] SpawnPoints;
	public int spawnIndex;
	// Use this for initialization
	void Awake () {
		if ((Pyerus == null) && (GameObject.FindGameObjectWithTag("Player") != null)) 
		{
			Pyerus = GameObject.FindGameObjectWithTag("Player");
		} else {
			Debug.LogWarning ("WHERE IS PYERUS. SHE IS NOT HERE. PLS FIX");
		}
	}

	void Start ()
	{
		SpawnPyerus ();
	}
		

	void MovePyerus () {
		if ((0 <= spawnIndex) && (spawnIndex <= SpawnPoints.Length) && (SpawnPoints [spawnIndex] != null)) {
			Pyerus.transform.position = SpawnPoints [spawnIndex].transform.position;
			Pyerus.transform.rotation = SpawnPoints [spawnIndex].transform.rotation;
			print ("Pyerus spawned at Index " + spawnIndex + " at " + SpawnPoints [spawnIndex].transform.position.x + ", " + SpawnPoints [spawnIndex].transform.position.y + ", " + SpawnPoints [spawnIndex].transform.position.z);
		} else {
			Debug.LogError ("Spawn is out of range or does not exist!");
		}
	}
	public void SpawnPyerus () { //uses global variable spawnPointIndexLoad to determine spawn point index to put pyerus
		if ((0 <= GlobalVariables.spawnPointIndexLoad) && (GlobalVariables.spawnPointIndexLoad <= SpawnPoints.Length) && (SpawnPoints [GlobalVariables.spawnPointIndexLoad] != null)) {
			Pyerus.transform.position = SpawnPoints [GlobalVariables.spawnPointIndexLoad].transform.position;
			Pyerus.transform.rotation = SpawnPoints [GlobalVariables.spawnPointIndexLoad].transform.rotation;
			print ("Pyerus spawned at Index " + GlobalVariables.spawnPointIndexLoad + " at "+ SpawnPoints [GlobalVariables.spawnPointIndexLoad].transform.position.x + ", " + SpawnPoints [GlobalVariables.spawnPointIndexLoad].transform.position.y + ", " + SpawnPoints [GlobalVariables.spawnPointIndexLoad].transform.position.z);
		} else {
			MovePyerus ();
		}
	}

}
