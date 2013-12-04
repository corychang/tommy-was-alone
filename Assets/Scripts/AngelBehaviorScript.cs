// Inspiration from http://www.red3d.com/cwr/steer/gdc99/

using UnityEngine;
using System.Collections;

public class AngelBehaviorScript : MonoBehaviour {
	// Constant variables
	public const float ACCELERATION = 40.0f;
	public const float CHARACTER_WIDTH = 1.0f;
	public const float FLEE_CHARACTERS_WEIGHT = 20.0f;
	public const float FLEE_OBJECT_MAX_DISTANCE = 60.0f;
	public const float FLEE_OBJECT_SCALE = 1600.0f;
	public const float FLEE_OBJECT_WEIGHT = 20.0f;
	public const float FLOCK_NEAR_DISTANCE = 30.0f;
	public const float FLOCK_SCALE = 80.0f;
	public const float MAX_TIME = 20.0f;
	public const float PROPORTION = 0.1f;
	public const float REACH_GOAL_SLOWING_DISTANCE = 50.0f;
	
	// Public variables
	public static GameObject player = null;
	//public GameObject goalRef = null;
	public ThirdPersonController[] allChars = null;
	public float maxSpeed = 20.0f;
	//public int characterNumber = 1;
	
	// Private variables
	// TODO: Set this based on the number of characters on the screen.
	private Vector3[] oldDirections = new Vector3[1];

	public void Start() {
		if (player == null) {
			player = GameObject.Find("Player");
		}
	}
	
	public void UpdateDesiredVelocity(ThirdPersonController c) {
		// Run twice to reduce jittering.
		Vector3 acc1 = ComputeDesiredVelocity(c);
		Vector3 acc2 = ComputeDesiredVelocity(c);
		Vector3 acc = (acc1 + acc2) / 2.0f;

		if (!acc.Equals(Vector3.zero)) {
			// v = v0 + at
			Vector3 newVelocity = c.moveSpeed * c.GetDirection() + acc * Time.deltaTime;
			// Some floating point error sometimes causes characters to walk to the sky.
			if (newVelocity.y != 0)
				newVelocity.y = 0;
			c.moveSpeed = newVelocity.magnitude;
			c.SetDirection(newVelocity.normalized);
		} else {
			c.moveSpeed = 0.0f;
		}
	}
	
	// Return the seek acceleration.
	public Vector3 ComputeDesiredVelocity(ThirdPersonController c) {
		oldDirections[c.characterNumber] = c.GetDirection();
		Vector3 acc = Vector3.zero;
		if (player != null) {
			Vector3 direction = c.transform.position - player.transform.position;
			// TODO: Make sure this is the correct direction the player is facing.
			float dotProduct = Vector3.Dot(direction, player.transform.forward);
			if (dotProduct < 0.0f) {
				acc = 30.0f * behaviorSeek(c, player.transform.position);
			} else {
				return Vector3.zero;
			}
		} else {
			return Vector3.zero;
		}
		Vector3 fleeObjs = fleeObjects(c, acc);
		// Flocking already accounts for separation.
		//Vector3 fleeChars = desiredScene == LevelConfigurationScript.Scene.FlockingWander ? Vector3.zero : fleeCharacters(c);
		//acc += FLEE_OBJECT_WEIGHT * fleeObjs + FLEE_CHARACTERS_WEIGHT * fleeChars;
		acc += FLEE_OBJECT_WEIGHT * fleeObjs;
		// Prevent accelerations from getting too large.
		return Mathf.Min(acc.magnitude, ACCELERATION) * acc.normalized;
	}
	
	// Push characters away from obstacles.
	public Vector3 fleeObjects(ThirdPersonController c, Vector3 acc) {
		RaycastHit hit;
		Vector3 ans = Vector3.zero;
		// Shoot rays from the center, left edge, and right edge of the character to find objects.
		Vector3 left = Vector3.Cross(c.GetDirection(), Vector3.up);
		if (Physics.Raycast(c.transform.position, c.GetDirection(), out hit, FLEE_OBJECT_MAX_DISTANCE)) {
			ans = FLEE_OBJECT_SCALE * hit.normal / (hit.distance * hit.distance);
		} else if (Physics.Raycast(c.transform.position + CHARACTER_WIDTH * left, c.GetDirection(), out hit, FLEE_OBJECT_MAX_DISTANCE)) {
			ans = FLEE_OBJECT_SCALE * hit.normal / (hit.distance * hit.distance);
		} else if (Physics.Raycast(c.transform.position - CHARACTER_WIDTH * left, c.GetDirection(), out hit, FLEE_OBJECT_MAX_DISTANCE)) {
			ans = FLEE_OBJECT_SCALE * hit.normal / (hit.distance * hit.distance);
		}
		return ans;
	}
	
	// Apply separations on all characters.
	public Vector3 fleeCharacters(ThirdPersonController c) {
		Vector3 sum = Vector3.zero;
		if (allChars == null)
			return sum;
		for (int i = 0; i < allChars.Length; i++) {
			// Skip comparisons of characters with themselves.
			if (c != allChars[i]) {
				// Compute time when characters are closest.
				Vector3 dp = allChars[i].transform.position - c.transform.position;
				Vector3 dv = oldDirections[i] - oldDirections[c.characterNumber];
				float t = -Vector3.Dot(dp, dv) / Vector3.Dot(dv, dv);
				// Only flee for small t.
				if (t > 0.0f && t < MAX_TIME) {
					// Run separation.
					Vector3 displacement = c.transform.position - allChars[i].transform.position;
					sum += displacement / displacement.sqrMagnitude;
				}
			}
		}
		return sum;
	}
	
	// Seek steering behavior. Used by wander.
	protected Vector3 behaviorSeek(ThirdPersonController c, Vector3 goal) {
		Vector3 displacement = goal - c.transform.position;
		Vector3 desiredVelocity = maxSpeed * displacement.normalized;
		return desiredVelocity - c.moveSpeed * c.GetDirection();
	}
	
	// Arrive steering behavior.
	protected Vector3 behaviorReachGoal(ThirdPersonController c, Vector3 goal) {
		Vector3 targetOffset = goal - c.transform.position;
		float distance = targetOffset.magnitude;
		float rampedSpeed = maxSpeed * (distance / REACH_GOAL_SLOWING_DISTANCE);
		float clampedSpeed = Mathf.Min(rampedSpeed, maxSpeed);
		Vector3 desiredVelocity = (clampedSpeed / distance) * targetOffset;
		return desiredVelocity - c.moveSpeed * c.GetDirection();
	}
}
