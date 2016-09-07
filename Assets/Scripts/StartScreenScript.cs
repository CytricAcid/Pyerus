using UnityEngine;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

	public GameObject startButton;
	public GameObject controlsButton;
	public GameObject controlsDialogue;

	// Use this for initialization
	public void GotoGame () {
		Application.LoadLevel (0);
	}
	
	// Update is called once per frame
	public void GotoControlMenu () {
		startButton.SetActive (false);
		controlsButton.SetActive (false);
		controlsDialogue.SetActive (true);
	}

	public void GotoMainMenu () {
		startButton.SetActive (true);
		controlsButton.SetActive (true);
		controlsDialogue.SetActive (false);
	}
}
