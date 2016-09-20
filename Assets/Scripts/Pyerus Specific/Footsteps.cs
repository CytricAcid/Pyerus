using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	public AudioClip[] footsteps;
	public AudioClip jumpSound;

	public float footstepVolume;
	public float jumpVolume;

	private AudioSource source;
	// Use this for initialization
	void Awake () {

		source = GetComponent<AudioSource>();
	}

	void FootstepSounds ()
	{
		source.PlayOneShot (footsteps[Random.Range(0,footsteps.Length)],footstepVolume);
	}

	public void JumpSound ()
	{
		source.PlayOneShot (jumpSound, jumpVolume);
	}

}
