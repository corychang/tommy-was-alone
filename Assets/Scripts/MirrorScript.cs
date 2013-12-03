using UnityEngine;
using System.Collections;

public class MirrorScript : MonoBehaviour {
	public GameObject mirror;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mirrPos = mirror.transform.position;
		mirrPos.z = -1 * mirrPos.z;
		gameObject.transform.position = mirrPos;

		/*
		Vector3 mirrAngles = mirror.transform.rotation.eulerAngles;
		mirrAngles.y = -1 * mirrAngles.y;
		gameObject.transform.rotation = mirrAngles.;
		*/
		//gameObject.transform.rotation.SetLookRotation(mirror.gameObject.transform.forward);
		Quaternion rot = mirror.transform.rotation;
		rot.y = rot.y * -1;
		gameObject.transform.rotation = rot;
		//gameObject.transform.rotation.y = mirror.transform.rotation.y * -1;
	}
}
