using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
	AudioClip clip;
	WorldLogic worldLogic;
	WorldSide currentSide;
	List<GameObject> currentAudioSources;
	AudioSource musicSource;
	AudioSource narrationSource;
	GameObject player;

	// Use this for initialization
	void Start () {
		// TODO handle music better
		clip = Resources.Load<AudioClip>("Music/A_BackHall");
		musicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
		musicSource.clip = clip; 
		musicSource.Play();
		narrationSource = GameObject.Find("NarrationSource").GetComponent<AudioSource>();
		worldLogic = GameObject.Find("world").GetComponent<WorldLogic>();
		currentSide = worldLogic.CurrentSide;
		setNarrationSources();
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		//first check if we're on a new side
		if(currentSide != worldLogic.CurrentSide)
		{
			currentSide = worldLogic.CurrentSide;
			setNarrationSources(); //update narration sources
		}
		// now loop through the narration sources and check to see if we're near any of them
		// if so, play the appropriate audio
		GameObject foundNarrationPoint = null;
		foreach(GameObject source in currentAudioSources)
		{
			float minZ = source.transform.position.z - source.transform.lossyScale.z/2.0f;
			float maxZ = source.transform.position.z + source.transform.lossyScale.z/2.0f;
			float minX = source.transform.position.x - source.transform.lossyScale.x/2.0f;
			float maxX = source.transform.position.x + source.transform.lossyScale.x/2.0f;
			//now see if the player is inside these bounds
			float playerX = player.transform.position.x;
			float playerZ = player.transform.position.z;
			if(minX <= playerX && playerX <= maxX && minZ <= playerZ && playerZ <= maxZ)
			{
				foundNarrationPoint = source;
				break; //found a narration point, break from loop
			}
		}
		if(foundNarrationPoint != null) //found a narration to play
		{
			// assuming foundNarrationPoint's name is a number 1,2,etc
			string resourcePath = "Narration/narration_" + foundNarrationPoint.name;
			narrationSource.clip = Resources.Load<AudioClip>(resourcePath); //check if null?
			narrationSource.Play(); //play the narration
			currentAudioSources.Remove(foundNarrationPoint);
			GameObject.Destroy(foundNarrationPoint);
		}
	}

	void setNarrationSources()
	{
		string tag;
		switch(currentSide)
		{
		case WorldSide.Side1:
			tag = "level1";
			break;
		case WorldSide.Side2:
			tag = "level2";
			break;
		case WorldSide.Side3:
			tag = "level3";
			break;
		case WorldSide.Side4:
			tag = "level4";
			break;
		case WorldSide.Side5:
			tag = "level5";
			break;
		case WorldSide.Side6:
			tag = "level6";
			break;
		default:
			throw new UnityException("Unknown side");
		}
		currentAudioSources = new List<GameObject>(GameObject.FindGameObjectsWithTag(tag));
	}
}
