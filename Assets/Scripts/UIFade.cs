using UnityEngine;
using System.Collections;

public class UIFade : MonoBehaviour {

	public bool isTransparent;
	public bool isFinished = false;
	public string levelToLoad;
	// Use this for initialization
	public void FadeIn () {
		StartCoroutine (DoFadeIn ());	
	}

	public void FadeOut () {
		StartCoroutine (DoFadeOut ());	
	}

	IEnumerator DoFadeIn () {
		CanvasGroup canvasGroup = GetComponent<CanvasGroup> ();
		while (canvasGroup.alpha > 0) {
			canvasGroup.alpha -= Time.deltaTime;
			yield return null;
		}
		isTransparent = true;
		canvasGroup.interactable = false;
		yield return null;
	}

	IEnumerator DoFadeOut () {
		CanvasGroup canvasGroup = GetComponent<CanvasGroup> ();
		while (canvasGroup.alpha < 1) {
			canvasGroup.alpha += Time.deltaTime;
			yield return null;
		}
		isTransparent = false;
		canvasGroup.interactable = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene (levelToLoad);
		yield return null;
	}
}
