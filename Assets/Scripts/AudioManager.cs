using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	AudioClip clip;

	// Use this for initialization
	void Start () {
		clip = Resources.Load<AudioClip>("Music/A_BackHall");
		AudioSource source = GameObject.Find("Music").GetComponent<AudioSource>();
		source.clip = clip;
		source.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
