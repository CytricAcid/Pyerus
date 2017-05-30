using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour {
	public float xSpeed;
	public float ySpeed;
	public float zSpeed;

	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(xSpeed,ySpeed,zSpeed) * Time.deltaTime);
	}
}
