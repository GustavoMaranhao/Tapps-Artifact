using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptFarmingStart : MonoBehaviour {
	public float moveToCenterSpeed = 10f;

	private GameObject playerRef;

	private Camera mainPlayerCamera;
	private Camera mainFarmCamera;

	void Start () {
		playerRef = ScriptGameManager.playerRef;

		mainPlayerCamera = ScriptGameManager.playerCameraRef;
		mainFarmCamera = ScriptGameManager.farmCameraRef;
	}

	void Update () {
		if (ScriptGameManager.bStartMinigame && ScriptGameManager.bStuck) {
			// If the player is not there yet, disconsider the height
			if ((!Mathf.Approximately (playerRef.transform.position.x, gameObject.transform.position.x)) ||
				(!Mathf.Approximately (playerRef.transform.position.z, gameObject.transform.position.z))) {
				//Create a new vector ignoring height
				Vector3 auxLocation = new Vector3(gameObject.transform.position.x,playerRef.transform.position.y,gameObject.transform.position.z);
				// Move player to the center of the plot
				playerRef.transform.position = Vector3.Lerp (playerRef.transform.position, auxLocation, moveToCenterSpeed * Time.deltaTime);
			}

			if (playerRef.transform.rotation != new Quaternion (0, 0, 0, 0))
				playerRef.transform.rotation = new Quaternion (0, 0, 0, 0);
		}

		if (ScriptGameManager.bStartMinigame) {
			Vector3 spawnPos = new Vector3 (playerRef.transform.position.x - 5f, playerRef.transform.position.y + 0.7f, playerRef.transform.position.z + 0.2f);

			if (!ScriptGameManager.bCubeSpawned) 			
				ScriptGameManager.SpawnContainer (spawnPos);
			else
				ScriptGameManager.containerRef.transform.position = spawnPos;
		}
	}

	void OnTriggerEnter(Collider col){
		if ((col.transform.tag == "Player") && ScriptGameManager.bQuestStarted){
			mainPlayerCamera.enabled = false;
			mainPlayerCamera.GetComponent<AudioListener> ().enabled = false;

			mainFarmCamera.enabled = true;
			mainPlayerCamera.GetComponent<AudioListener> ().enabled = true;

			playerRef.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;

			ScriptGameManager.currentCamera = mainFarmCamera;
			ScriptGameManager.bStuck = true;
			ScriptGameManager.bStartMinigame = true;
		}
	}
}
