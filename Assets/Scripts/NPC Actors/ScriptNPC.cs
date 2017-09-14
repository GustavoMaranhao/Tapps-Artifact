using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptNPC : MonoBehaviour {
	public float distanceActivation = 5.0f;
	public GameObject pupText;

	public float xOffset = 50f;
	public float yOffest = 50f;

	public float jumpSpeed = 60f;

	private GameObject player;
	private bool bActivated = false;
	private bool bVictoryActivated = false;

	private Rigidbody cubeRB;

	void Start () {
		player = ScriptGameManager.playerRef;
		cubeRB = gameObject.GetComponent<Rigidbody> ();

		pupText = ScriptGameManager.NPCPupText;
		ScriptGameManager.newPupText = "Olá! Tenho uma tarefa para você! Preciso de ajuda coletando alguns recursos. Os recursos estão na fazenda do outro lado do rio.";
	}

	void Update () {
		//Check if the player is close enough to show the message
		if (Vector3.Distance (player.transform.position, gameObject.transform.position) <= distanceActivation) {
			//If the message isn't active yet, activate it
			if (!bActivated) {				
				pupText.SetActive (true);
				bActivated = true;
			}
			//Activate the quest variable if it isn't yet
			if (!ScriptGameManager.bQuestStarted)
				ScriptGameManager.bQuestStarted = true;
		} else {
			//Tell the game the container moved away
			ScriptGameManager.bContainerIsCloseToNPC = false;
			if (bActivated) {
				//If the player has just moved away, deactivate the message and change what it says and the cube isn't spawned
				if (!ScriptGameManager.bCubeSpawned)
					ScriptGameManager.newPupText = "Ainda preciso de ajuda coletando aqueles recursos!";
				pupText.SetActive (false);
				bActivated = false;
				// All constraints except Y are reset, preparing if the player falls in the river again
				if (player.GetComponent<Rigidbody> ().constraints == ~RigidbodyConstraints.FreezePositionY)
					player.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			}
		}

		//Check if the container is already set
		if (ScriptGameManager.containerRef != null) {	
			//Check if the container is close enough disconsidering the height
			Vector3 tempContainerPos = ScriptGameManager.containerRef.transform.position;
			tempContainerPos.y = 0;
			Vector3 tempNPCPos = gameObject.transform.position;
			tempNPCPos.y = 0;
			if (Vector3.Distance (tempContainerPos, tempNPCPos) <= distanceActivation) {
				//Tell the game the container moved close
				ScriptGameManager.bContainerIsCloseToNPC = true;

				//Check if the player has brought the container close enough with something, the message isn't active and the quest is active
				if (!bActivated && ScriptGameManager.bQuestStarted && ScriptGameManager.containerRef.GetComponent<ScriptCubeContainer> ().bHasSomething)
					ScriptGameManager.newPupText = "Muito bem! Traga-o até aqui, quero ver o que você me trouxe!";
			} else
				//Tell the game the container moved away
				ScriptGameManager.bContainerIsCloseToNPC = false;
		}

		//Set the message in the correct position above the NPC
		if (pupText.activeInHierarchy) {
			Vector3 wantedPos = Camera.main.WorldToViewportPoint (gameObject.transform.position);
			pupText.transform.position = new Vector3 (wantedPos.x * Screen.width + xOffset, wantedPos.y * Screen.height + yOffest, 40);
		}

		//Check if the player accomplished his objective
		if (ScriptGameManager.bVictory) {
			//Make the little cube jump
			float height = cubeRB.transform.position.y;
			if ((cubeRB.velocity.magnitude <= 2f) && (height <= 1.2f))
				cubeRB.AddForce (new Vector3 (0,jumpSpeed,0));

			//Activate the victory message
			if (!bVictoryActivated) {
				ScriptGameManager.newPupText = "Woohoo! Você conseguiu!"; 
				pupText.SetActive (true);
				cubeRB.isKinematic = false;

				bVictoryActivated = true;
			}
		}
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "CubeContainer" && ScriptGameManager.bContainerIsCloseToNPC) {
			col.gameObject.transform.rotation = new Quaternion (0, 0, 0, 0);
			col.gameObject.GetComponent<Collider> ().enabled = false;
			col.gameObject.GetComponent<Rigidbody> ().AddForce (0, 1000f, 0);
			col.gameObject.GetComponent<ScriptObjectFade> ().bStartDissolving = true;
		}
	}
}
