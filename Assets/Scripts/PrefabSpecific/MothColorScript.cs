using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothColorScript : MonoBehaviour {
	Texture2D mothTexture;
	public GameObject Moth;
	Color glowColor;
	// Use this for initialization
	void Start () {
		//mothTexture = (Texture2D)GetComponentInChildren<Renderer> ().material.mainTexture;
		mothTexture = (Texture2D)Moth.GetComponentInChildren<Renderer> ().material.mainTexture;
		glowColor = mothTexture.GetPixel (0, 0);

		ChangeColors ();
		}





	// Call this to change the colors of the particle effects and lights
	void ChangeColors () {
		Component[] particleEffects;
		particleEffects = GetComponentsInChildren (typeof(ParticleSystem), true);
		if (particleEffects != null) {
			foreach (ParticleSystem glowy in particleEffects) {
				var aaa = glowy.main; //idk why unity wants me to do this just to access start color but it does
				aaa.startColor = glowColor;
			}
		} else {
			Debug.LogWarning ("There are no particle effects on this object!");
		}
		Light mothLight = GetComponentInChildren (typeof(Light), true).GetComponent<Light>();
		mothLight.color = glowColor;

	}
}
