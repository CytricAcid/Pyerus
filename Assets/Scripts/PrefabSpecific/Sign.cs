using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour {
	CanvasGroup CtrlButton;
	CanvasGroup DialogueBox;
	GameObject TextBox;
	public string DialogueText;

	// Use this for initialization

	void Awake () {
		if ((CtrlButton == null) && (GameObject.FindGameObjectWithTag("UI") != null)) 
		{
			CtrlButton = GameObject.Find ("PressCtrl").GetComponent<CanvasGroup> ();
			DialogueBox = GameObject.Find ("DialogueBox").GetComponent<CanvasGroup> ();
			TextBox = GameObject.Find ("Text");

		} else {
			Debug.LogWarning ("No GUI in scene!");
		}
	}
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") 
		{
			CtrlButton.GetComponent<CanvasGroup>().alpha = 1f;
			TextBox.GetComponent<Text> ().text = DialogueText;
		}
	}

	void OnTriggerStay (Collider other) { //When Pyerus is in the Area of the sign
		if (other.tag == "Player") {
			if (other.GetComponent<PlayerInputController> ().Current.ActionInput) {
				DialogueBox.alpha = 1f;
				CtrlButton.alpha = 0f;

			}
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.tag == "Player") 
		{
			CtrlButton.alpha = 0f;
			DialogueBox.alpha = 0f;
		}
	}

}

