using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	public AudioClip[] footsteps;
	public AudioClip jumpSound;

	private AudioSource source;
	// Use this for initialization
	void Awake () {

		source = GetComponent<AudioSource>();
	}

	void FootstepSounds (float volume)
	{
		source.PlayOneShot (footsteps[Random.Range(0,footsteps.Length)],volume);
	}

	public void JumpSound (float volume)
	{
		source.PlayOneShot (jumpSound, volume);
	}

}
