using UnityEngine;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

	// Use this for initialization
	public void GotoGame () {
		Application.LoadLevel (0);
	}
	
	// Update is called once per frame
	public void GotoControlMenu () {
		Application.LoadLevel (1);
	}

	public void GotoMainMenu () {
		Application.LoadLevel (2);
	}
}
