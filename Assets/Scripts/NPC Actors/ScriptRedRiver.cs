using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptRedRiver : MonoBehaviour {
	private GameObject Lid;
	private GameObject LeftWall;
	private GameObject RightWall;

	void Start() {
		Transform[] ts = gameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) {
			if (t.gameObject.name == "RiverLeftWall") {
				LeftWall = t.gameObject;
				continue;
			}
			if (t.gameObject.name == "RiverRightWall") {
				RightWall = t.gameObject;
				continue;
			}
			if (t.gameObject.name == "RiverLid") {
				Lid = t.gameObject;
				continue;
			}
		}
	}

	void Update(){
		if (ScriptGameManager.bQuestStarted && !ScriptGameManager.bPathOpen) {
			ScriptGameManager.bPathOpen = true;
			LeftWall.GetComponent<ScriptObjectFade> ().bStartDissolving = true;
			LeftWall.GetComponent<ScriptObjectFade> ().bFadeOut = true;

			RightWall.GetComponent<ScriptObjectFade> ().bStartDissolving = true;
			RightWall.GetComponent<ScriptObjectFade> ().bFadeOut = true;

			Lid.GetComponent<ScriptObjectFade> ().bStartDissolving = true;
			Lid.GetComponent<ScriptObjectFade> ().bFadeOut = true;
		}
	}
	
	void OnCollisionEnter(Collision col){
		if (col.transform.tag == "Player") {		
			ScriptGameManager.newPupText = "Cuidado para você não cair no rio por engano!";
			ScriptGameManager.PlayerRespawn();
			col.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		} else {
			if ((col.transform.tag == "CubeContainer") || (col.transform.tag == "finishedCollectible")){
				//ScriptGameManager.PlayerWon ();
				col.transform.GetComponent<Rigidbody> ().AddForce (new Vector3 (7500f, 0, 0));
			}
		}
	}
}
