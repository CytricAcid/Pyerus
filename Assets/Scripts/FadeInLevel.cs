using UnityEngine;
using System.Collections;

public class FadeInLevel : MonoBehaviour {
	public GameObject UI;
	UIFade UIFadeObject;
	// Use this for initialization
	void Awake () {
			if ((UIFadeObject == null) && (UI.GetComponent<UIFade> () != null)) {
				UIFadeObject = UI.GetComponent<UIFade> ();
			} else {
				Debug.LogWarning ("Missing UI Fade script!!!!!!1111");
			}
		UIFadeObject.FadeIn ();
	}

}
