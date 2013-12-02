using UnityEngine;
using System.Collections;

public class WorldLogic : MonoBehaviour {
	public float gravity = 20.0f; //universal gravity for the world
	GameObject player;
	const float EPSILON = 1.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}

	// Update simply checks the players position. If he's moved "off the cube"
	// then we rotate the cube by 90 degrees and shift the player
	void Update () {
		Vector3 temp_pos = transform.position;
		bool cube_rotated = false;
		if(player.transform.position.x < transform.position.x - transform.lossyScale.x/2.0f)
		{
			this.transform.Rotate(new Vector3(0.0f,0.0f,-90.0f));
			temp_pos.x -= (transform.localScale.x - EPSILON);
			cube_rotated = true;
		}
		else if(player.transform.position.x > transform.position.x + transform.lossyScale.x/2.0f)
		{
			this.transform.Rotate(new Vector3(0.0f,0.0f,90.0f));
			temp_pos.x += (transform.localScale.x - EPSILON);
			cube_rotated = true;
		}
		else if(player.transform.position.z < transform.position.z - transform.lossyScale.z/2.0f)
		{
			this.transform.Rotate(new Vector3(90.0f,0.0f,0.0f));
			temp_pos.z -= (transform.localScale.z - EPSILON);
			cube_rotated = true;
		}
		else if(player.transform.position.z > transform.position.z + transform.lossyScale.z/2.0f)
		{
			this.transform.Rotate(new Vector3(-90.0f,0.0f,0.0f));
			temp_pos.z += (transform.localScale.z - EPSILON);
			cube_rotated = true;
		}
		if(cube_rotated)
		{
			player.transform.position = new Vector3(player.transform.position.x, 
			                                        player.transform.position.y + 5.0f,
			                                        player.transform.position.z);
			
			transform.position = temp_pos;
		}
	}
}
