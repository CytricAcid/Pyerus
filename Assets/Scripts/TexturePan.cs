using UnityEngine;
using System.Collections;

public class TexturePan : MonoBehaviour {

	public float scrollSpeed;
	public Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
	}

	void Update() 
	{
		float offset = Time.time * scrollSpeed;
		rend.material.mainTextureOffset = new Vector2(offset, 0);
	}
}
