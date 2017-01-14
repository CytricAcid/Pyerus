using UnityEngine;
using System.Collections;
using AC;

public class PyerusTrails : MonoBehaviour {

	public GameObject PlayerTarget;    

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
