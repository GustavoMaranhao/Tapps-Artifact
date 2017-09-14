using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUIMoveSprite : MonoBehaviour {
	[HideInInspector]
	public bool bShouldExpand = false;

	[HideInInspector]
	public Vector3 startPosition;
	[HideInInspector]
	public Vector3 endPosition;

	[HideInInspector]
	public float expandSpeed = 1750.0f;

	private float startTime;
	private float journeyLength;

	public void StartJourney () {
		//Tell the child to expand
		bShouldExpand = true;

		startTime = Time.time;
		journeyLength = Vector3.Distance (startPosition, endPosition);
	}
	
	// Update is called once per frame
	void Update () {
		//If the player just clicked the collectibles pannel
		if (bShouldExpand) {			
			//Move the images
			float distCovered = (Time.time - startTime) * expandSpeed;
			float fracJourney = distCovered / journeyLength;

			transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

			//If they arrived at the final position, increase the counter
			if (transform.position == endPosition)
				bShouldExpand = false;
		}		
	}
}
