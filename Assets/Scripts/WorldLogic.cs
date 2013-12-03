using UnityEngine;
using System.Collections;

public class WorldLogic : MonoBehaviour {
	public float gravity = 20.0f; //universal gravity for the world
	GameObject player;
	const float EPSILON = 1.0f;
	Vector3 playerPositionCache;
	bool shouldCachePosition = false;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}

	// Update simply checks the players position. If he's moved "off the cube"
	// then we rotate the cube by 90 degrees and shift the player
	void Update () {
		if(shouldCachePosition)
		{
			shouldCachePosition = false;
			player.transform.position = playerPositionCache;
		}
		Vector3 temp_pos = transform.position;
		int rotateCase = -1;
		if(player.transform.position.x < transform.position.x - transform.lossyScale.x/2.0f)
		{
			this.transform.Rotate(new Vector3(0.0f,0.0f,-90.0f), Space.World);
			//temp_pos.x -= (transform.localScale.x - EPSILON);
			temp_pos.x -= transform.lossyScale.x;
			rotateCase = 0;
		}
		else if(player.transform.position.x > transform.position.x + transform.lossyScale.x/2.0f)
		{
			this.transform.Rotate(new Vector3(0.0f,0.0f,90.0f), Space.World);
			//temp_pos.x += (transform.localScale.x - EPSILON);
			temp_pos.x += transform.lossyScale.x;
			rotateCase = 1;
		}
		else if(player.transform.position.z < transform.position.z - transform.lossyScale.z/2.0f)
		{
			this.transform.Rotate(new Vector3(90.0f,0.0f,0.0f), Space.World);
			//temp_pos.z -= (transform.localScale.z - EPSILON);
			temp_pos.z -= transform.lossyScale.z;
			rotateCase = 2;
		}
		else if(player.transform.position.z > transform.position.z + transform.lossyScale.z/2.0f)
		{
			this.transform.Rotate(new Vector3(-90.0f,0.0f,0.0f), Space.World);
			//temp_pos.z += (transform.localScale.z - EPSILON);
			temp_pos.z += transform.lossyScale.z;
			rotateCase = 3;
		}
		if(rotateCase != -1)
		{
			CharacterMotor cm = player.GetComponent<CharacterMotor>();
			cm.movement.velocity = new Vector3(0.0f,0.0f,0.0f);
			player.transform.position = new Vector3(player.transform.position.x, 
			                                        player.transform.position.y + 1.0f,
			                                        player.transform.position.z);
			Vector3 player_temp = player.transform.position;
			switch (rotateCase)
			{
				case 0:
					player_temp.x -= EPSILON;
					break;
				case 1:
					player_temp.x += EPSILON;
					break;
				case 2:
					player_temp.z -= EPSILON;
					break;
				case 3:
					player_temp.z += EPSILON;
					break;
				default:
					break;
			}
			player.transform.position = player_temp;
			playerPositionCache = player_temp;
			this.shouldCachePosition = true;
			
			transform.position = temp_pos;
		}
	}
}
