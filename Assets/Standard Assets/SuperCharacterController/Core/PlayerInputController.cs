using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public PlayerInput Current;

	// Use this for initialization
	void Start () {
        Current = new PlayerInput();
	}
	
	// Update is called once per frame
	void Update () {
        
        // Retrieve our current WASD or Arrow Key input
        // Using GetAxisRaw removes any kind of gravity or filtering being applied to the input
        // Ensuring that we are getting either -1, 0 or 1
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        bool jumpInput = Input.GetButtonDown("Jump");
		bool jumpHeldInput = Input.GetButton ("Jump");
		bool actionInput = Input.GetButtonDown ("Fire1");

        Current = new PlayerInput()
        {
            MoveInput = moveInput,
            MouseInput = mouseInput,
            JumpInput = jumpInput,
			JumpHeldInput = jumpHeldInput,
			ActionInput = actionInput
        };
	}
}

public struct PlayerInput
{
    public Vector3 MoveInput;
    public Vector2 MouseInput;
    public bool JumpInput;
	public bool JumpHeldInput;
	public bool ActionInput;
}
