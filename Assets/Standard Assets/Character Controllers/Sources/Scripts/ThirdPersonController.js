
// Require a character controller to be attached to the same game object
@script RequireComponent(CharacterController)

public var idleAnimation : AnimationClip;
public var walkAnimation : AnimationClip;
public var runAnimation : AnimationClip;
public var jumpPoseAnimation : AnimationClip;

public var walkMaxAnimationSpeed : float = 0.75;
public var trotMaxAnimationSpeed : float = 1.0;
public var runMaxAnimationSpeed : float = 1.0;
public var jumpAnimationSpeed : float = 1.15;
public var landAnimationSpeed : float = 1.0;

public var angel : boolean = false;

private var _animation : Animation;

enum CharacterState {
	Idle = 0,
	Walking = 1,
	Trotting = 2,
	Running = 3,
	Jumping = 4,
}

private var _characterState : CharacterState;

// The speed when walking
var walkSpeed = 2.0;
// after trotAfterSeconds of walking we trot with trotSpeed
var trotSpeed = 4.0;
// when pressing "Fire3" button (cmd) we start running
var runSpeed = 6.0;

var inAirControlAcceleration = 3.0;

// How high do we jump when pressing jump and letting go immediately
var jumpHeight = 0.5;

// The gravity for the character
var gravity = 20.0;
// The gravity in controlled descent mode
var speedSmoothing = 10.0;
var rotateSpeed = 500.0;
var trotAfterSeconds = 3.0;

var canJump = false;

var characterNumber = 1;

private var jumpRepeatTime = 0.05;
private var jumpTimeout = 0.15;
private var groundedTimeout = 0.25;

// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
private var lockCameraTimer = 0.0;

// The current move direction in x-z
private var moveDirection = Vector3.zero;
// The current vertical speed
private var verticalSpeed = 0.0;
// The current x-z move speed
public var moveSpeed = 0.0;

// The last collision flags returned from controller.Move
private var collisionFlags : CollisionFlags; 

// Are we jumping? (Initiated with jump button and not grounded yet)
private var jumping = false;
private var jumpingReachedApex = false;

// Are we moving backwards (This locks the camera to not do a 180 degree spin)
private var movingBack = false;
// Is the user pressing any keys?
private var isMoving = false;
// When did the user start walking (Used for going into trot after a while)
private var walkTimeStart = 0.0;
// Last time the jump button was clicked down
private var lastJumpButtonTime = -10.0;
// Last time we performed a jump
private var lastJumpTime = -1.0;


// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
private var lastJumpStartHeight = 0.0;


private var inAirVelocity = Vector3.zero;

private var lastGroundedTime = 0.0;

private var isControllable = true;

function Awake ()
{
	moveDirection = transform.TransformDirection(Vector3.forward);
	
	_animation = GetComponent(Animation);
	if(!_animation)
		Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
	if(!idleAnimation) {
		_animation = null;
		Debug.Log("No idle animation found. Turning off animations.");
	}
	if(!walkAnimation) {
		_animation = null;
		Debug.Log("No walk animation found. Turning off animations.");
	}
	if(!runAnimation) {
		_animation = null;
		Debug.Log("No run animation found. Turning off animations.");
	}
	if(!jumpPoseAnimation && canJump) {
		_animation = null;
		Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
	}
}	

function Update() {
	// Call the UpdateDesiredVelocity method in the Behavior.cs file.
	var otherScript : Component;
	if (angel)
		otherScript = gameObject.GetComponent("AngelBehaviorScript");
	else
		otherScript = gameObject.GetComponent("BehaviorScript");
	otherScript.UpdateDesiredVelocity(this);
	
	// Calculate actual motion
	var movement = moveDirection * moveSpeed + Vector3 (0, verticalSpeed, 0) + inAirVelocity;
	movement *= Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	collisionFlags = controller.Move(movement);
	
	// ANIMATION sector
	if(_animation) {
		if(controller.velocity.sqrMagnitude < 0.1) {
			_animation.CrossFade(idleAnimation.name);
		}
		else 
		{
			if(_characterState == CharacterState.Running) {
				_animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
				_animation.CrossFade(runAnimation.name);	
			}
			else if(_characterState == CharacterState.Trotting) {
				_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, trotMaxAnimationSpeed);
				_animation.CrossFade(walkAnimation.name);	
			}
			else if(_characterState == CharacterState.Walking) {
				_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, walkMaxAnimationSpeed);
				_animation.CrossFade(walkAnimation.name);	
			}
			else if(_characterState == CharacterState.Idle) { //  Start walking again when moving.
				_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, walkMaxAnimationSpeed);
				_animation.CrossFade(walkAnimation.name);
			}
		}
	}
	// ANIMATION sector
	
	// Set rotation to the move direction
	transform.rotation = Quaternion.LookRotation(moveDirection);
}

function OnControllerColliderHit (hit : ControllerColliderHit )
{
//	Debug.DrawRay(hit.point, hit.normal);
	if (hit.moveDirection.y > 0.01) 
		return;
}

function GetSpeed () {
	return moveSpeed;
}

function IsJumping () {
	return jumping;
}

function IsGrounded () {
	return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
}

function GetDirection () {
	return moveDirection;
}

function SetDirection (d) {
	moveDirection = d;
}

function IsMovingBackwards () {
	return movingBack;
}

function GetLockCameraTimer () 
{
	return lockCameraTimer;
}

function IsMoving ()  : boolean
{
	return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
}

function HasJumpReachedApex ()
{
	return jumpingReachedApex;
}

function IsGroundedWithTimeout ()
{
	return lastGroundedTime + groundedTimeout > Time.time;
}

function Reset ()
{
	gameObject.tag = "Player";
}