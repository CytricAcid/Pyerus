using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

	public Transform AnimatedMesh;

	public float WalkSpeed = 4.0f;
	public float WalkAcceleration = 30.0f;
	public float JumpAcceleration = 5.0f;
	public float JumpHeight = 3.0f;
	public float DoubleJumpHeight = 0.5f;
	public float Gravity = 25.0f;
	public float Friction = 20.0f;
	public float GlideSpeed = -0.3f;
	private float defaultGravity;
	public float inputDecay { get; private set; }
	private bool doubleJump;


	Animator animator;

	// Animation variables

	// Add more states by comma separating them
	enum PlayerStates { Idle, Walk, Jump, Fall, DoubleJump, Glide }

	private SuperCharacterController controller;

	// current velocity
	public Vector3 moveDirection { get; private set; }
	// current direction our character's art is facing
	public Vector3 lookDirection { get; private set; }

	private PlayerInputController input;

	void Start () {
		// Put any code here you want to run ONCE, when the object is initialized
		inputDecay = 0f;

		input = gameObject.GetComponent<PlayerInputController>();

		// Grab the controller object from our object
		controller = gameObject.GetComponent<SuperCharacterController>();

		// Our character's current facing direction, planar to the ground
		lookDirection = transform.forward;

		// Set our currentState to idle on startup
		currentState = PlayerStates.Idle;

		animator = gameObject.GetComponent<Animator> ();

		defaultGravity = Gravity;
	}

	protected override void EarlyGlobalSuperUpdate()
	{
		// Rotate out facing direction horizontally based on mouse input
		lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x * (controller.deltaTime / Time.deltaTime), controller.up) * lookDirection;

		if (inputDecay != 0f)
		{
			inputDecay -= controller.deltaTime;
			if (inputDecay <= 0f)
			{
				inputDecay = 0f;
			}
		}
		// Put any code in here you want to run BEFORE the state's update function.
		// This is run regardless of what state you're in
	}

	public float getVelocity()
	{
		return ((Mathf.Pow (moveDirection.x, 2)) + (Mathf.Pow (moveDirection.z, 2)));
	}

	protected override void LateGlobalSuperUpdate()
	{
		// Put any code in here you want to run AFTER the state's update function.
		// This is run regardless of what state you're in
		animator.SetInteger ("State", System.Convert.ToInt32(currentState));

		// Move the player by our velocity every frame
		transform.position += moveDirection * controller.deltaTime;

		// Rotate our mesh to face where we are "looking"
		if (moveDirection.x != 0 && moveDirection.z != 0) {
			AnimatedMesh.rotation = Quaternion.LookRotation ((Math3d.ProjectVectorOnPlane (controller.up, moveDirection)), controller.up);
		}

		float trueVelocity = ((Mathf.Pow (moveDirection.x, 2)) + (Mathf.Pow (moveDirection.z, 2)));
		animator.SetFloat ("Speed", trueVelocity);
	}

	private bool AcquiringGround()
	{
		return controller.currentGround.IsGrounded(false, 0.05f);
	}

	private bool MaintainingGround()
	{
		return controller.currentGround.IsGrounded(true, 0.05f);
	}

	public void RotateGravity(Vector3 up)
	{
		lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
	}

	/// <summary>
	/// Constructs a vector representing our movement local to our lookDirection, which is
	/// controlled by the camera
	/// </summary>
	private Vector3 LocalMovement()
	{
		Vector3 right = Vector3.Cross(controller.up, lookDirection);

		Vector3 local = Vector3.zero;

		if (input.Current.MoveInput.x != 0)
		{
			local += right * input.Current.MoveInput.x;
		}

		if (input.Current.MoveInput.z != 0)
		{
			local += lookDirection * input.Current.MoveInput.z;
		}

		return local.normalized;
	}

	// Calculate the initial velocity of a jump based off gravity and desired maximum height attained
	private float CalculateJumpSpeed(float jumpHeight, float gravity)
	{
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	/*void Update () {
	 * Update is normally run once on every frame update. We won't be using it
     * in this case, since the SuperCharacterController component sends a callback Update 
     * called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires
     * further callbacks depending on the state
	}*/

	// Below are the three state functions. Each one is called based on the name of the state,
	// so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
	// Jump_SuperUpdate()

	/// <summary>
	/// Idle
	/// </summary>

	void Idle_EnterState()
	{
		controller.EnableSlopeLimit();
		controller.EnableClamping();
		doubleJump = false;
	}

	void Idle_SuperUpdate()
	{
		// Run every frame we are in the idle state

		if (input.Current.JumpInput)
		{
			currentState = PlayerStates.Jump;
			return;
		}

		if (!MaintainingGround())
		{
			currentState = PlayerStates.Fall;
			return;
		}

		if (input.Current.MoveInput != Vector3.zero)
		{
			currentState = PlayerStates.Walk;
			return;
		}

		// Apply friction to slow us to a halt
		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, Friction * controller.deltaTime); 
	}

	void Idle_ExitState()
	{
		// Run once when we exit the idle state
	}

	/// <summary>
	/// Walking
	/// </summary>

	void Walk_SuperUpdate()
	{
		if (input.Current.JumpInput)
		{
			currentState = PlayerStates.Jump;
			return;
		}

		if (!MaintainingGround())
		{
			currentState = PlayerStates.Fall;
			return;
		}

		if (input.Current.MoveInput != Vector3.zero)
		{
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * WalkSpeed, WalkAcceleration * controller.deltaTime);
		}
		else
		{
			currentState = PlayerStates.Idle;
			return;
		}
	}

	/// <summary>
	/// Jumping
	/// </summary>

	void Jump_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
		inputDecay = ( ((CalculateJumpSpeed(JumpHeight, Gravity) / (Gravity)) / 4));
	}

	void Jump_SuperUpdate()
	{

		if (input.Current.JumpInput && doubleJump == false && inputDecay == 0f)
		{
			currentState = PlayerStates.DoubleJump;
			return;
		}
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
		{
			moveDirection = planarMoveDirection;
			currentState = PlayerStates.Idle;
			return;            
		}

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
		verticalMoveDirection.y = Mathf.Max (verticalMoveDirection.y, -7f);

		moveDirection = planarMoveDirection + verticalMoveDirection;
	}

	/// <summary>
	/// Double Jump
	/// </summary>

	void DoubleJump_EnterState()
	{
		moveDirection = controller.up * CalculateJumpSpeed(DoubleJumpHeight, Gravity);
		inputDecay = ( ((CalculateJumpSpeed(DoubleJumpHeight, Gravity) / (Gravity)) + .1f )); //calculates the exact time the double jump reaches its apex
		doubleJump = true;
	}

	void DoubleJump_SuperUpdate()
	{
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
		{
			moveDirection = planarMoveDirection;
			currentState = PlayerStates.Idle;
			return;            
		}

		if (input.Current.JumpHeldInput && inputDecay == 0f)
		{
			currentState = PlayerStates.Glide;
			return;
		}

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
		verticalMoveDirection.y = Mathf.Max (verticalMoveDirection.y, -7f);

		moveDirection = planarMoveDirection + verticalMoveDirection;
	}

	/// <summary>
	/// Falling
	/// </summary>

	void Fall_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		// moveDirection = trueVelocity;
	}

	void Fall_SuperUpdate()
	{
		if (AcquiringGround())
		{
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			currentState = PlayerStates.Idle;
			return;
		}

		if (input.Current.JumpInput && doubleJump == false)
		{
			currentState = PlayerStates.DoubleJump;
			return;
		}

		if (input.Current.JumpInput && doubleJump == true)
		{
			currentState = PlayerStates.Glide;
			return;
		}

		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
		verticalMoveDirection.y = Mathf.Max (verticalMoveDirection.y, -7f);

		moveDirection = planarMoveDirection + verticalMoveDirection;

	}

	/// <summary>
	/// Glide
	/// </summary>


	void Glide_SuperUpdate()
	{
		if (AcquiringGround())
		{
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			currentState = PlayerStates.Idle;
			return;
		}
		if (!input.Current.JumpHeldInput)
		{
			currentState = PlayerStates.Fall;
			return;
		}
			

		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * WalkSpeed, JumpAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
		verticalMoveDirection.y = Mathf.Max (verticalMoveDirection.y, GlideSpeed);

		moveDirection = planarMoveDirection + verticalMoveDirection;


	}
		
}
