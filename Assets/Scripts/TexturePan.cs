using UnityEngine;
using System.Collections;

public class TexturePan : MonoBehaviour {

	public float scrollSpeedX;
    public float scrollSpeedY;
	public Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
	}

	void Update() 
	{
		float offset = Time.time * scrollSpeedX;
        float offset2 = Time.time * scrollSpeedY;
		rend.material.mainTextureOffset = new Vector2(offset, offset2);
	}
}
