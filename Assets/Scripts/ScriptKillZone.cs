using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptKillZone : MonoBehaviour {
	void OnTriggerEnter(Collider col){
		if ((col.transform.tag == "CubeContainer") || (col.transform.tag == "finishedCollectible")) {
			ScriptGameManager.newPupText = "Ainda preciso de ajuda coletando aqueles recursos!";
			ScriptGameManager.bCubeSpawned = false;

			if (col.transform.tag == "finishedCollectible")
				ScriptGameManager.NotifyNewCollectibleAdded (col.name);
		}
		
		Destroy (col.gameObject);
	}
}
