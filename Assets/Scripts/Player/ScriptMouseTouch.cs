using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMouseTouch : MonoBehaviour {
	[HideInInspector]
	public static Vector3 worldPoint;
	[HideInInspector]
	public static RaycastHit cursorRayHit;

	private Camera mainCamera;
	private float camDistance;

	private Ray cursorRay;

	// RayCasting ignoring all layers except "Terrain" (number 8) or "FarmObjects" (number 9), it will collide with 1s and ignore 0s
	private static int ignoreLayerMask = (1 << 8) + (1 << 9);

	void Start () {
		mainCamera = ScriptGameManager.currentCamera;
	}

	void Update () {
		if (mainCamera != ScriptGameManager.currentCamera) 
			mainCamera = ScriptGameManager.currentCamera;

		// Get the starting point of the ray
		if (Input.touchCount > 0)
			cursorRay = mainCamera.ScreenPointToRay (Input.GetTouch (0).position);
		else
			cursorRay = mainCamera.ScreenPointToRay(Input.mousePosition);

		//Get the camera distance to the touch
		camDistance = mainCamera.gameObject.transform.localPosition.magnitude + 20;

		// Do the raycast
		if (Physics.Raycast (cursorRay, out cursorRayHit, (int)camDistance, ignoreLayerMask)) {
			worldPoint = cursorRay.GetPoint (cursorRayHit.distance);
		}
	}
}
