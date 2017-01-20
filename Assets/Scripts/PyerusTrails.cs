using UnityEngine;
using System.Collections;

public class PyerusTrails : MonoBehaviour {

	public GameObject PlayerTarget;    //eventually replace this with a superstate machine that gets the player automatically

	private PlayerMachine machine;

	private TrailRenderer trail;


	// Use this for initialization
	void Start () {
		machine = PlayerTarget.GetComponent<PlayerMachine>();
		trail = GetComponent<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(System.Convert.ToInt32(machine.currentState) == 5 && machine.getVelocity() > 0f)
		{
			trail.enabled = true;
		}
		else
		{
			trail.enabled = false;
		}
	}
}
