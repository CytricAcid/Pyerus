using UnityEngine;
using System.Collections;

public class FadeToLoadLevel : MonoBehaviour {

	public string levelToLoad;
	public int spawnPointIndexSet;
	public GameObject UI;
	UIFade UIFadeObject;
	void Start() {
		if ((UIFadeObject == null) && (UI.GetComponent<UIFade> () != null)) {
			UIFadeObject = UI.GetComponent<UIFade> ();
		} else {
			Debug.LogWarning ("Missing UI Fade script!!!!!!1111");
		}
		//DontDestroyOnLoad ();
	}

	void OnTriggerEnter(Collider other) {
		UIFadeObject.FadeOut ();
		other.GetComponent<SuperCharacterController> ().enabled = false;
	}
	void Update ()
	{
		if (UIFade.isFinished == true)
		{
			GlobalVariables.spawnPointIndexLoad = spawnPointIndexSet;
			UnityEngine.SceneManagement.SceneManager.LoadScene (levelToLoad);
			UIFade.isFinished = false;
		}
	}
}
