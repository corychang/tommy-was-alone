using UnityEngine;
using System.Collections;

public class OutlineSpawner : MonoBehaviour {
	public GameObject outlinePrefab;
	private int cubeCount = 3;
	private int cubeScaleJump = 1;
	private int cubeScaleInit = 5;
	private GameObject[] cubes;
	// Use this for initialization
	void Start () {
		cubes = new GameObject[cubeCount];
		int curScale = cubeScaleInit;
		for (int i = 0; i < cubeCount; i++)
		{
			GameObject newOutline = (GameObject)Instantiate (outlinePrefab, Vector3.zero, Quaternion.identity);
			newOutline.transform.localScale = new Vector3(curScale, curScale, curScale);
			curScale += cubeScaleJump;
			cubes[i] = newOutline;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < cubeCount; i++)
		{
			//cubes[i].transform.Rotate (new Vector3(cubeCount - i, 0, 0));
			cubes[i].transform.Rotate((new Vector3(0,cubeCount -i,0)) * Time.deltaTime * Mathf.Min (Time.realtimeSinceStartup / 10, 1.0f));
		}
	}
}
