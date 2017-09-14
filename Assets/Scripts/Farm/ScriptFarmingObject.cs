using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptFarmingObject : MonoBehaviour {
	[HideInInspector]
	public bool bCanPickup = true;

	public float moveBackSpeed = 5f;

	private bool bDroppedCorrectly = false;
	private bool bDroppedOutside = false;

	private bool bShouldMoveBack = false;
	private Vector3 originalPosition;

	private bool bChangeScale = false;
	private bool bResourceReady = false;

	private GameObject cubeContainer;

	private Color originalColor;

	void Start (){
		originalPosition = gameObject.transform.position;
		originalColor = gameObject.GetComponent<Renderer> ().material.color;
	}

	void Update () {
		//Check if the resource can be picked
		if (bResourceReady) {
			//Check if the object was not already used
			if (bCanPickup) {
				//Drop the object
				if (Input.GetMouseButtonUp (0)) {
					//If player let go, check if it was dropped on the cube container
					if (bDroppedCorrectly) {
						//Attach it to the container
						bCanPickup = false;
						gameObject.SetActive (false);

						//Change the container's color and size
						Vector3 newScale = cubeContainer.transform.localScale + new Vector3 (0.1f, 0.1f, 0.1f);
						cubeContainer.transform.localScale = newScale;
						cubeContainer.GetComponent<ScriptColorBlink> ().AddColor (gameObject.GetComponent<Renderer> ().material.color);

						//Add to the stats of the current container
						ScriptFarmingResources resourcesScript = gameObject.GetComponentInParent<ScriptFarmingResources> ();
						ScriptCubeContainer stats = cubeContainer.GetComponent<ScriptCubeContainer> ();
						stats.bHasSomething = true;
						if ((resourcesScript.spawnObjects[0] != null) && (gameObject.name.Contains(resourcesScript.spawnObjects [0].name)))
							stats.contType1++;
						else if ((resourcesScript.spawnObjects[1] != null) && (gameObject.name.Contains(resourcesScript.spawnObjects [1].name)))
							stats.contType2++;
						else if ((resourcesScript.spawnObjects[2] != null) && (gameObject.name.Contains(resourcesScript.spawnObjects [2].name)))
							stats.contType3++;
						else if ((resourcesScript.spawnObjects[3] != null) && (gameObject.name.Contains(resourcesScript.spawnObjects [3].name)))
							stats.contType4++;

						//Add to the counter of the game manager
						ScriptGameManager.objectsCollected++;
					} else {	
						//Check where the object was dropped
						if (bDroppedOutside)
						//Move the object back where it was
						bShouldMoveBack = true;
					}

					ScriptGameManager.objectBeingDragged = null;
				}
				if ((Input.touchCount > 0) && (Input.GetTouch (0).phase == TouchPhase.Ended)) {
					//If player let go, check if it was dropped on the cube container
					if (bDroppedCorrectly) {
						//Attach it to the container
						bCanPickup = false;
						gameObject.SetActive (false);
						//Always add the lighter material to the color
						cubeContainer.GetComponent<ScriptColorBlink> ().AddColor (gameObject.GetComponent<ScriptColorBlink> ().ColorStart);
						Vector3 newScale = cubeContainer.transform.localScale + new Vector3 (0.1f, 0.1f, 0.1f);
						cubeContainer.transform.localScale = newScale;
					} else {
						//Check where the object was dropped
						if (bDroppedOutside)
							//Move the object back where it was
							bShouldMoveBack = true;
					}

					ScriptGameManager.objectBeingDragged = null;
				}
			}


			//Move the ovject back to the previous location
			if (bShouldMoveBack) {
				gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, originalPosition, moveBackSpeed * Time.deltaTime);
				if (Mathf.Approximately (gameObject.transform.position.magnitude, originalPosition.magnitude))
					bShouldMoveBack = false;
			}
			
			//If we are in the minigame, there is something under the cursor, we didnt get anything yet and it wasnt dropped in the container yet
			if ((ScriptMouseTouch.cursorRayHit.transform != null) && ScriptGameManager.bStartMinigame &&
			   (ScriptGameManager.objectBeingDragged == null) && bCanPickup) {
				//Check if the object is this one
				if (ScriptMouseTouch.cursorRayHit.transform.gameObject == this.gameObject) 
				//If the player clicked
				if (((Input.touchCount > 0) && (Input.GetTouch (0).phase == TouchPhase.Began)) || Input.GetMouseButtonDown (0)) {
					//Assign this object to be moved
					ScriptGameManager.objectBeingDragged = gameObject;	
					
					//Get the position where the object was
					originalPosition = gameObject.transform.position;

					//Reset the variable
					bShouldMoveBack = false;

					//Stop the blinking
					gameObject.GetComponent<ScriptColorBlink>().bStopBlinking = true;
					gameObject.GetComponent<Renderer> ().material.color = originalColor;
				}
			}

			//If this is the object assigned, drag it together with the cursor
			if (ScriptGameManager.objectBeingDragged == gameObject) {
				Vector3 auxLoc = ScriptMouseTouch.worldPoint;
				auxLoc.y = gameObject.transform.position.y;
				gameObject.transform.position = auxLoc;

				//Check if the object is still within its parent tile
				Vector3 parentPos = gameObject.transform.parent.position;
				float parentWidth = gameObject.transform.parent.GetComponent<ScriptFarmingResources> ().farmWidth;
				float parentHeight = gameObject.transform.parent.GetComponent<ScriptFarmingResources> ().farmHeight;
				if (((gameObject.transform.position.x > parentPos.x + parentWidth / 2) || (gameObject.transform.position.x < parentPos.x - parentWidth / 2)) ||
				   ((gameObject.transform.position.z > parentPos.z + parentHeight / 2) || (gameObject.transform.position.z < parentPos.z - parentHeight / 2)))
					bDroppedOutside = true;
				else
					bDroppedOutside = false;
			}
		}

		//Should start the cube growing
		if (bChangeScale) {
			//Disable picking up while resource is growing
			bResourceReady = false;

			//Make the object grow depending on the speed set in the parent
			float scaleSpeed = gameObject.transform.parent.GetComponent<ScriptFarmingResources> ().spawnSpeed;
			Vector3 newScale = gameObject.transform.localScale + new Vector3 (scaleSpeed*0.1f, scaleSpeed*0.1f, scaleSpeed*0.1f);
			gameObject.transform.localScale = newScale;

			//Check if the resourse is at the maximum size
			if (newScale.x >= 1f) {
				//Notify that the resource has finished growing
				bResourceReady = true;
				bChangeScale = false;
				gameObject.transform.parent.GetComponent<ScriptFarmingResources> ().bSpawnNext = true;

				//Create a lighter color of the current one
				Color endColor = gameObject.GetComponent<Renderer>().material.color;
				endColor = new Color (endColor.r/2, endColor.g/2, endColor.b/2);

				//Blink the resource once
				gameObject.GetComponent<ScriptColorBlink>().ColorStart = gameObject.GetComponent<Renderer>().material.color;
				gameObject.GetComponent<ScriptColorBlink> ().ColorEnd = endColor;
				gameObject.GetComponent<ScriptColorBlink>().enabled = true;
			}
		}			
	}

	public void ChangeScale(){
		//Prepare the scale change and notify to start it
		gameObject.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f);
		bChangeScale = true;
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "CubeContainer") {
			if (!other.GetComponent<ScriptColorBlink> ().bColorOverflowing)
				bDroppedCorrectly = true;

			if (cubeContainer == null)
				cubeContainer = other.gameObject;
		}
	}

	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "CubeContainer")
			bDroppedCorrectly = false;
	}
}
