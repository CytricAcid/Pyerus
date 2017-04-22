using UnityEngine;
using System.Collections;

public class UIFade : MonoBehaviour {

	public bool isTransparent;
	public static bool isFinished = false;
	// Use this for initialization
	public void FadeIn () {
		StopAllCoroutines ();
		StartCoroutine (DoFadeIn ());	
	}

	public void FadeOut () {
		StopAllCoroutines ();
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
		isFinished = true;
		yield return null;
	}
}
