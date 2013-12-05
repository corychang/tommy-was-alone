using UnityEngine;
using System.Collections;

public class MirrorScript : MonoBehaviour {
	public GameObject mirror;
	public GameObject mirrorPiece;
	public Camera zoomOutCamera;
	public Camera mainCamera;
	public Light sceneLight;

	private Light[] mirrorPieceLights;
	private int numMirrorsWidth = 12;
	private int numMirrorsHeight = 15;
	private float mirrorSpacing = 2.0f;
	private float mirrorY = 1.0f;
	private bool spawnedMirror = false;
	private float fadeOutScale = 0.0f;
	private bool isFading;
	private float mirrorLightFade;
	private float maxIntensity;
	private float cameraZoomDelay = 0.0f;
	// Use this for initialization
	void Start () {
		spawnedMirror = false;
		isFading = false;
		fadeOutScale = 0.0f;
		mirrorLightFade = 0.0f;

		mirrorPieceLights = gameObject.GetComponentsInChildren<Light>();
		maxIntensity = mirrorPieceLights[0].intensity;
	}
	
	// Update is called once per frame
	void Update () {
		/*For the mirrored player (the zombie cube), mirror the player position/angle*/
		Vector3 mirrPos = mirror.transform.position;
		mirrPos.z = -1 * mirrPos.z;
		gameObject.transform.position = mirrPos;

		Quaternion rot = mirror.transform.rotation;
		rot.y = rot.y * -1;
		gameObject.transform.rotation = rot;

		/*Check to see if the player has hit the mirror yet, and spawn the pieces if so.*/

		if (mirror.transform.position.z > -10)
		{
			mirrorLightFade += Time.deltaTime;
			//mirrorLightFade = Mathf.Min (1.0f, mirrorLightFade);
			for (int a = 0; a < mirrorPieceLights.GetLength(0); a++)
			{
				Light curLight = mirrorPieceLights[a];
				curLight.intensity = maxIntensity * (1.0f - mirrorLightFade);
			}
		}

		if (mirror.transform.position.z > -10 && !spawnedMirror && mirrorLightFade >= 2.0f)
		{
			spawnedMirror = true;
			isFading = true;
			spawnMirrorPieces();
		}

		if (isFading)
		{
			fadeOutScale += Time.deltaTime / 3.0f;
			fadeOutScale = Mathf.Min (1.0f, fadeOutScale);
			Color curCol = renderer.material.color;;
			curCol.a = 1.0f - fadeOutScale;
			renderer.material.color = curCol;
			sceneLight.intensity = (fadeOutScale - 0.1f) / 2.0f;
			cameraZoomDelay += Time.deltaTime;
		}

		if (cameraZoomDelay > 10.0f)
		{

			zoomOutCamera.depth = 1;
			mainCamera.depth = 0;

			Vector3 pos = zoomOutCamera.transform.position;
			pos.y = pos.y + ((cameraZoomDelay - 10.0f) * Time.deltaTime * 2.0f);
			zoomOutCamera.transform.position = pos;
		}
	}

	void spawnMirrorPieces()
	{
		for (int i = 0; i < numMirrorsHeight; i++)
			for (int j = 0; j < numMirrorsWidth; j++)
			{
				Instantiate (mirrorPiece, new Vector3(j*mirrorSpacing, mirrorY + (i*mirrorSpacing), 0), Quaternion.identity);
				if (j != 0)
					Instantiate (mirrorPiece, new Vector3(-1.0f * j*mirrorSpacing, mirrorY + (i*mirrorSpacing), 0), Quaternion.identity);
			}
	}
}
