using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCubeContainer : MonoBehaviour {
	[HideInInspector]
	public bool bHasSomething = false;
	[HideInInspector]
	public bool bDissolve = false;

	[HideInInspector]
	public int contType1 = 0;
	[HideInInspector]
	public int contType2 = 0;
	[HideInInspector]
	public int contType3 = 0;
	[HideInInspector]
	public int contType4 = 0;

	public float percDistanceToDisappearFromCamera = 200f;

	private ScriptColorBlink blinkScript;
	private ScriptObjectFade fadeScript;

	void Start() {
		blinkScript = gameObject.GetComponent<ScriptColorBlink> ();
		fadeScript = gameObject.GetComponent<ScriptObjectFade> ();
	}

	void Update() {
		//If we are out of the minigame, the object is active and still blinking, stop it
		if (!ScriptGameManager.bStartMinigame && gameObject.activeInHierarchy && !blinkScript.bStopBlinking)
			blinkScript.bStopBlinking = true;

		//Check if we should dissolve
		if (bDissolve)
			fadeScript.bStartDissolving = bDissolve;

		//Check if the cube is outside the player's viewrange
		Vector3 cubeCamPos = ScriptGameManager.playerCameraRef.WorldToViewportPoint(gameObject.transform.position);
		//If neither the x and y coordinates are between 0 and 1 (and the z coordinate is positive), we are outside the minigame and player is close
		if (!((cubeCamPos.x > -percDistanceToDisappearFromCamera) && (cubeCamPos.x < (1+percDistanceToDisappearFromCamera)) && 
			  (cubeCamPos.y > -percDistanceToDisappearFromCamera) && (cubeCamPos.y < (1+percDistanceToDisappearFromCamera)) && 
			  (cubeCamPos.z > 0)) && !ScriptGameManager.bStartMinigame && ScriptGameManager.bContainerIsCloseToNPC) {
			//Creaate a random collectible
			GameObject newItem = ScriptGameManager.scriptGameManagerRef.ChooseNewCollectible(contType1,contType2,contType3,contType4);

			//If the player didn't get it right, spawn the container again;
			if (newItem == null) {
				ScriptGameManager.SpawnContainer (gameObject.transform.position);
				ScriptGameManager.newPupText = "Infelizmente não posso fazer nada com estes materiais, será que você poderia tentar novamente?";
			} else {
				ScriptGameManager.newPupText = "Muito bom! Est"+newItem.name+" era um dos itens que eu estava procurando!";

				newItem = Instantiate (newItem, gameObject.transform.position, Quaternion.identity) as GameObject;
				newItem.tag = "finishedCollectible";

				//Color all the child components and the parent one
				newItem.GetComponent<Renderer> ().material.color = gameObject.GetComponent<Renderer> ().material.color;
				Component[] childrenRenderers = newItem.GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer rend in childrenRenderers)
					rend.material.color = gameObject.GetComponent<Renderer> ().material.color;

				//Let the game know we are done using this cube
				//ScriptGameManager.bCubeSpawned = false;
			}

			//Destroy the previous container object
			Destroy (gameObject);
		}			
	}
}
