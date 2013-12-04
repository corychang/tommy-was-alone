using UnityEngine;
using System.Collections;

public enum WorldSide
{
	Side1,
	Side2,
	Side3,
	Side4,
	Side5,
	Side6
}

public class WorldLogic : MonoBehaviour {
	enum Delta
	{
		PosX,
		NegX,
		PosZ,
		NegZ
	}

	public float gravity = 20.0f; //universal gravity for the world
	GameObject player;
	const float EPSILON = 1.0f;
	Vector3 playerPositionCache;
	bool shouldCachePosition = false;
	WorldSide currentSide;
	public WorldSide CurrentSide
	{
		get {return currentSide;}
	}

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
					currentSide = computeNewSide(Delta.NegX);
					break;
				case 1:
					player_temp.x += EPSILON;
					currentSide = computeNewSide(Delta.PosX);
					break;
				case 2:
					player_temp.z -= EPSILON;
					currentSide = computeNewSide(Delta.NegZ);
					break;
				case 3:
					player_temp.z += EPSILON;
					currentSide = computeNewSide(Delta.PosZ);
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
	/// <summary>
	/// Helper to update world side. Exploits the fact that you can only have
	/// certain transitions based on the structure of the world
	/// </summary>
	/// <returns>The rotation.</returns>
	/// <param name="delta">Delta.</param>
	WorldSide computeNewSide(Delta delta)
	{
		switch(currentSide)
		{
		case WorldSide.Side1:
			if(delta == Delta.PosZ)
				return WorldSide.Side2;
			break;
		case WorldSide.Side2:
			if(delta == Delta.NegZ)
				return WorldSide.Side1;
			if(delta == Delta.PosZ)
				return WorldSide.Side3;
			break;
		case WorldSide.Side3:
			if(delta == Delta.NegZ)
				return WorldSide.Side2;
			if(delta == Delta.PosZ)
				return WorldSide.Side6;
			if(delta == Delta.PosX)
				return WorldSide.Side5;
			if(delta == Delta.NegX)
				return WorldSide.Side4;
			break;
		case WorldSide.Side4:
			if(delta == Delta.PosX)
				return WorldSide.Side3;
			break;
		case WorldSide.Side5:
			if(delta == Delta.NegX)
				return WorldSide.Side3;
			break;
		case WorldSide.Side6:
			if(delta == Delta.NegZ)
				return WorldSide.Side3;
			break;
		default:
			throw new UnityException("unknown WorldSide");
		}
		throw new UnityException("Invalid transition on cube side");
	}
}
