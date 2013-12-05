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
	AudioSource soundEffectSource;
	GameObject player;
	string current_tag =""; //tag for current sources

	// Use this for initialization
	void Start () {
		// TODO handle music better
		clip = Resources.Load<AudioClip>("Music/A_BackHall");
		musicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
		musicSource.clip = clip; 
		musicSource.Play();
		narrationSource = GameObject.Find("NarrationSource").GetComponent<AudioSource>();
		soundEffectSource = GameObject.Find("SoundEffectSource").GetComponent<AudioSource>();
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
		if(currentAudioSources.Count == 0)
			return; //no more update to do
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
		if(current_tag == "levelb1" || current_tag == "levelb2")
		{
			// special hack for the b1 and b2 case as we have redundant
			// narration to allow player to do it in any order
			GameObject.Destroy(currentAudioSources[0]);
			currentAudioSources.RemoveAt(0);
		}
	}

	void setNarrationSources()
	{
		switch(currentSide)
		{
		case WorldSide.Side1:
			current_tag = "level1";
			break;
		case WorldSide.Side2:
			current_tag = "level2";
			break;
		case WorldSide.Side3:
			current_tag = "level3";
			if(worldLogic.Button1Pressed^worldLogic.Button2Pressed)
				current_tag = "levelb1";
			if(worldLogic.Button1Pressed && worldLogic.Button2Pressed)
				current_tag = "levelb2";
			break;
		case WorldSide.Side4:
			current_tag = "level4";
			break;
		case WorldSide.Side5:
			current_tag = "level5";
			break;
		case WorldSide.Side6:
			current_tag = "level6";
			break;
		default:
			throw new UnityException("Unknown side");
		}
		currentAudioSources = new List<GameObject>(GameObject.FindGameObjectsWithTag(current_tag));
	}

	public void playSoundEffect(string effectPath)
	{
		soundEffectSource.clip = Resources.Load<AudioClip>(effectPath);
		soundEffectSource.Play();
	}
}
