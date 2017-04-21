using UnityEngine;
using System.Collections;

public class FadeToLoadLevel : MonoBehaviour {

	public string sceneToLoad;
	public GameObject UI;
	UIFade UIFadeObject;
	void Start() {
		if ((UIFadeObject == null) && (UI.GetComponent<UIFade> () != null)) {
			UIFadeObject = UI.GetComponent<UIFade> ();
		} else {
			Debug.LogWarning ("Missing UI Fade script!!!!!!1111");
		}
	}

	void OnTriggerEnter(Collider other) {
		UIFadeObject.levelToLoad = sceneToLoad;
		UIFadeObject.FadeOut ();
		other.GetComponent<PlayerInputController> ().enabled = false;
	}
}
