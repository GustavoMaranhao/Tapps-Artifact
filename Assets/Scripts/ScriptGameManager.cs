using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptGameManager : MonoBehaviour {
	[HideInInspector]
	public static ScriptGameManager scriptGameManagerRef;

	[HideInInspector]
	public static bool bStuck = false;
	[HideInInspector]
	public static bool bVictory = false;
	[HideInInspector]
	public static bool bRespawn = false;
	[HideInInspector]
	public static bool bStartMinigame = false;
	[HideInInspector]
	public static bool bCubeSpawned = false;
	[HideInInspector]
	public static bool bQuestStarted = false;
	[HideInInspector]
	public static bool bPathOpen = false;
	[HideInInspector]
	public static bool bContainerIsCloseToNPC = false;

	[HideInInspector]
	public static GameObject playerRef;
	[HideInInspector]
	public static GameObject NPCRef;
	[HideInInspector]
	public static GameObject containerRef;

	[HideInInspector]
	public static Camera playerCameraRef;
	[HideInInspector]
	public static Camera farmCameraRef;
	[HideInInspector]
	public static Camera currentCamera;

	[HideInInspector]
	public static GameObject canvasRef;
	[HideInInspector]
	public static GameObject NPCPupText;
	[HideInInspector]
	public static string newPupText = "";

	[HideInInspector]
	public static GameObject objectBeingDragged;
	[HideInInspector]
	public static int objectsCollected = 0;
	[HideInInspector]
	public static int collectiblesNumber = 5;

	public float fadeSpeed = 0.03f;
	public Material darkMaterial;
	public GameObject victoryText;
	public Vector3 playerRespawnPoint = new Vector3 (4.5f, 2.3f, 6.3f);

	public GameObject[] availableCollectibles;

	private bool bFinishedTransition = false;

	private GameObject fadePanel;
	private UnityEngine.UI.Text pupTextWords;
	private UnityEngine.CanvasGroup panelCanvasGroup;

	private List<GameObject> farmTiles = new List<GameObject>();
	public static ScriptUI ScriptUIRef;

	void Awake () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;	

		scriptGameManagerRef = this;

		playerRef = GameObject.FindGameObjectWithTag ("Player");
		NPCRef = GameObject.FindGameObjectWithTag ("NPC");

		playerCameraRef = playerRef.transform.GetChild(0).GetComponent<Camera>();
		farmCameraRef = GameObject.Find ("Main Camera Farm").GetComponent<Camera>();
		currentCamera = playerCameraRef;

		canvasRef = GameObject.Find ("Canvas");
		NPCPupText = GameObject.FindGameObjectWithTag ("popUpText");
		fadePanel = GameObject.Find ("fadePanel");

		pupTextWords = NPCPupText.GetComponent<UnityEngine.UI.Text> ();
		panelCanvasGroup = fadePanel.GetComponent<UnityEngine.CanvasGroup> ();

		ScriptUIRef = gameObject.GetComponent<ScriptUI> ();

		NPCPupText.SetActive (false);

		foreach (GameObject farm in GameObject.FindGameObjectsWithTag("farmTile")) {
			farmTiles.Add (farm as GameObject);
		}
	}
		
	void Update () {		
		CheckRespawnTransition ();

		if (newPupText != "") {
			pupTextWords.text = newPupText;
			newPupText = "";
		}

		if (bVictory)
			victoryText.SetActive (true);
	}

	void CheckRespawnTransition(){
		if (bRespawn) {
			panelCanvasGroup.alpha += fadeSpeed;

			if (panelCanvasGroup.alpha >= 1) {
				bRespawn = false;
				bFinishedTransition = true;
			}
		}

		if (bFinishedTransition) {
			panelCanvasGroup.alpha -= fadeSpeed/3;

			playerRef.transform.position = playerRespawnPoint;
			playerRef.transform.rotation = new Quaternion (0, 0, 0, 0);

			if (panelCanvasGroup.alpha <= 0) {				
				bFinishedTransition = false;
				bStuck = false;
				// All constraints except for Y
				playerRef.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
				playerRef.GetComponent<Rigidbody> ().useGravity = true;
			}
		}
	}

	public static void PlayerRespawn(){				
		bRespawn = true;
	}

	public static void PlayerWon(){
		playerRef.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		bStuck = true;
		bVictory = true;
	}

	public void NotifyExitFarm(){
		foreach (GameObject farm in farmTiles) {
			farm.GetComponent<ScriptFarmingResources> ().DeactivateFarmObjects ();
		}
	}

	public static void SpawnContainer(Vector3 spawnPos){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = spawnPos;
		cube.AddComponent<Rigidbody> ();
		cube.AddComponent<ScriptCubeContainer>();
		cube.AddComponent<ScriptColorBlink>();
		cube.AddComponent<ScriptObjectFade>();
		cube.tag = "CubeContainer";

		objectsCollected = 0;
		bCubeSpawned  = true;
		containerRef = cube;
	}

	//For this game stat1 = capsule, stat2 = cylinder, stat3 = sphere, stat4 = cube
	//Car = 2xCube, 4xCylinder
	//Ghost = 3xCapsule, 1xSphere
	//Guitar = 1xCapsule, 2xCube, 2xCylinder
	//Plane = 1xCapsule, 4xCube
	//Pyramid = 9xCube
	public GameObject ChooseNewCollectible(int stat1, int stat2, int stat3, int stat4){
		GameObject newCollectible = null;

		//With 0 Capsules it can be the Car or the Pyramid
		if (stat1 == 0) {
			//With 2 Cubes it can only be the Car
			if ((stat2 == 4) && (stat3 == 0) && (stat4 == 2)) {				
				newCollectible = availableCollectibles [0];
				newCollectible.name = "e Carro";
			}
			//If different than 2 it must be the Pyramid
			else if ((stat2 == 0) && (stat3 == 0) && (stat4 == 9)) {
				newCollectible = availableCollectibles [4];
				newCollectible.name = "a Pirâmide";
			}
		} else 
			//With 1 Capsule it can be the Guitar or the Plane
			if (stat1 == 1) {
				//With 2 Cubes it can only be the Guitar
				if ((stat2 == 2) && (stat3 == 0) && (stat4 == 2)) {					
					newCollectible = availableCollectibles [2];
					newCollectible.name = "a Guitarra";
				}
						//If different than 2 it must be the Plane
				else if ((stat2 == 0) && (stat3 == 0) && (stat4 == 4)) {
					newCollectible = availableCollectibles [3];
					newCollectible.name = "e Avião";
				}
			} else 
				//With 3 Capsules it can only be the Ghost
				if ((stat1 == 3) && (stat2 == 0) && (stat3 == 1) && (stat4 == 0)) {
					newCollectible = availableCollectibles [1];
					newCollectible.name = "e Boneco";
			}

		return newCollectible;
	}

	public static void NotifyNewCollectibleAdded (string colName){
		//Get the part of the string after the space, this will be how we identify the button in the UI
		colName = colName.Substring(colName.IndexOf(" ")+1);//, colName.Length-1);
		colName = colName.Substring(0, colName.IndexOf("("));
		ScriptUIRef.GetComponent<ScriptUI> ().LightCollectible (colName);
	}
}
