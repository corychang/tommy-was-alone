using UnityEngine;
using System.Collections;

public enum ButtonID
{
	button1,
	button2,
	button3
}

public class ButtonScript : MonoBehaviour {
	public ButtonID buttonID;
	public GameObject player;
	public WorldLogic worldLogic;
	bool wasPressed = false;

	// Use this for initialization
	void Start () {
		worldLogic = GameObject.Find("world").GetComponent<WorldLogic>();
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(wasPressed)
			return;
		bool shouldUpdate = false;
		switch(buttonID)
		{
		case ButtonID.button1:
			shouldUpdate = worldLogic.CurrentSide == WorldSide.Side4;
			break;
		case ButtonID.button2:
			shouldUpdate = worldLogic.CurrentSide == WorldSide.Side5;
			break;
		case ButtonID.button3:
			shouldUpdate = worldLogic.CurrentSide == WorldSide.Side6;
			break;
		}
		if(!shouldUpdate)
			return;
		//otherwise see if the player walked onto us
		float minZ = transform.position.z - transform.lossyScale.z/2.0f;
		float maxZ = transform.position.z + transform.lossyScale.z/2.0f;
		float minX = transform.position.x - transform.lossyScale.x/2.0f;
		float maxX = transform.position.x + transform.lossyScale.x/2.0f;
		//now see if the player is inside these bounds
		float playerX = player.transform.position.x;
		float playerZ = player.transform.position.z;
		//get y distance from button to player centers
		float delta = Mathf.Abs((player.transform.position.y - player.transform.localScale.y/2.0f) -
		                        (transform.position.y + transform.lossyScale.y / 2.0f));
		if(minX <= playerX && playerX <= maxX && minZ <= playerZ && playerZ <= maxZ &&
		   delta < 1.1f)
		{
			wasPressed = true;
			string tag = "";
			switch(buttonID)
			{
			case ButtonID.button1:
				tag = "b1_path";
				break;
			case ButtonID.button2:
				tag = "b2_path";
				break;
			case ButtonID.button3:
				tag = "b3_path";
				break;
			}
			GameObject[] button_paths = GameObject.FindGameObjectsWithTag(tag);
			// now set each item to have the same shader
			Shader shader = Shader.Find("Particles/Alpha Blended Premultiply");
			renderer.material.shader = shader;
			foreach(GameObject path in button_paths)
			{
				path.renderer.material.shader = shader;
			}
			worldLogic.registerButtonPress(buttonID);
		}
	}
}
