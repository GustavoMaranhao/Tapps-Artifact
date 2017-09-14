using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUI : MonoBehaviour {
	[HideInInspector]
	public bool bDropDownExpanded = false;
	[HideInInspector]
	public float panelStartHeight;

	public GameObject cancelFarmButton;
	public GameObject farmCollectedText;
	public GameObject canvasCollectibles;
	public GameObject dropDownPanel;

	public float imageExpansionSpeed = 1.0f;

	private UnityEngine.UI.Button cancelFarmButtonComp;
	private string textForButton = "";
	private string textForFarmCollected;

	private bool bIsExpanded = false;

	private Object[] colSheet;
	private bool[] colCompleted = new bool[ScriptGameManager.collectiblesNumber];

	// Use this for initialization
	void Start () {
		//Check if we are playing in a mobile phone or the PC and adjust the cancel button shortcut text
		#if UNITY_EDITOR || UNITY_STANDALONE
			textForButton = "Sair (X)";			
		#endif
		//#elif
		#if UNITY_ANDROID
			textForButton = "Sair";
		#endif

		cancelFarmButtonComp = cancelFarmButton.GetComponent<UnityEngine.UI.Button> ();
		panelStartHeight = dropDownPanel.transform.position.y;
		textForFarmCollected = farmCollectedText.GetComponent<UnityEngine.UI.Text> ().text;

		//Load our sprite sheet
		colSheet = Resources.LoadAll("Textures/Collectibles",typeof(Sprite));

		//Initialize the victory array
		for (int i = 0; i < colCompleted.Length; i++)
			colCompleted [i] = false;
	}

	void Update () {
		//Activate the collectibles pannel if the quest was started
		if (ScriptGameManager.bQuestStarted && !canvasCollectibles.activeInHierarchy) {
			canvasCollectibles.SetActive (true);
		}

		//Activate the exit farming minigame button and the collection text if the player arrived at the farm
		if (ScriptGameManager.bStartMinigame && !cancelFarmButton.activeInHierarchy) {
			cancelFarmButton.GetComponentInChildren<UnityEngine.UI.Text> ().text = textForButton;
			cancelFarmButton.SetActive (true);

			//Setup the text for the label
			farmCollectedText.GetComponentInChildren<UnityEngine.UI.Text> ().text = textForFarmCollected + ScriptGameManager.objectsCollected.ToString();
			farmCollectedText.SetActive (true);
		}

		//If the cancel button is active
		if (cancelFarmButton.activeInHierarchy) {
			//Check if the shortcut button was pressed and transition its color to a pressed state
			if (Input.GetKeyDown (KeyCode.X)) 
				cancelFarmButton.GetComponent<UnityEngine.UI.Graphic> ().CrossFadeColor (cancelFarmButtonComp.colors.pressedColor, cancelFarmButtonComp.colors.fadeDuration, true, true);
		
			//Check if the shortcut button was released, transition its color back to a released state and execute the action
			if (Input.GetKeyUp (KeyCode.X)) {
				cancelFarmButton.GetComponent<UnityEngine.UI.Graphic> ().CrossFadeColor (cancelFarmButtonComp.colors.normalColor, cancelFarmButtonComp.colors.fadeDuration, true, true);
				cancelFarmButton.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
			}
		}	

		//If the collection text is active
		if (farmCollectedText.activeInHierarchy) {
			//Always update the text in case we added something
			farmCollectedText.GetComponentInChildren<UnityEngine.UI.Text> ().text = textForFarmCollected + ScriptGameManager.objectsCollected.ToString ();
		}
	}

	public void cancelClick() {
		ScriptGameManager.bStuck = false;
		ScriptGameManager.bStartMinigame = false;
		ScriptGameManager.farmCameraRef.enabled = false;
		ScriptGameManager.playerCameraRef.enabled = true;
		ScriptGameManager.currentCamera = ScriptGameManager.playerCameraRef;
		ScriptGameManager.playerRef.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		cancelFarmButton.SetActive (false);
		farmCollectedText.SetActive (false);
	}

	public void collectiblesClick(){
		//Get the scripts to set the movement
		ScriptUIMoveSprite[] scriptsMovement = canvasCollectibles.GetComponentsInChildren<ScriptUIMoveSprite>();

		//Get the root button child, it must always be the last one in the draw order
		Transform rootButton = canvasCollectibles.transform.GetChild(canvasCollectibles.transform.childCount - 1);

		//Iterate through all the children
		foreach (ScriptUIMoveSprite childImage in scriptsMovement) {
			//Skip the dropdown panel
			if (childImage.name == dropDownPanel.name)
				continue;

			//Notify the child to play the expanding animation only if it is not already doing it
			if (!childImage.bShouldExpand) {
				//The starting position will always be where the child is
				childImage.startPosition = childImage.transform.position;

				//If the collectibles panel is expanded, retract it
				if (bIsExpanded) {
					//The end position will be the root button
					childImage.endPosition = rootButton.position;

					//Reset the position of the drop down panel
					Vector3 newPos = new Vector3(rootButton.GetComponent<RectTransform> ().localPosition.x, panelStartHeight);
					dropDownPanel.GetComponent<RectTransform> ().position = newPos;
					//And unmark the expanded status
					bDropDownExpanded = false;
				}
				//If the collectibles panel is contracted, expand it
				else {
					//Get the position this child should be, it is at the end of its name
					int pos = int.Parse(childImage.name[childImage.name.Length - 1].ToString());
					//Get the sprites width
					float imageWidth = childImage.GetComponent<RectTransform>().rect.width;

					//The end position will be pos times the images width on the x axis
					childImage.endPosition = rootButton.position;
					childImage.endPosition.x +=  pos*imageWidth;
				}
				//Set the speed to expand
				childImage.expandSpeed = imageExpansionSpeed;

				//Tell the child to start its movement
				childImage.StartJourney();
			}
		}

		//Invert the status for the expansion of the pannel
		bIsExpanded = !bIsExpanded;
	}

	public void LightCollectible(string colName){		
		//Get the scripts to set the movement
		Transform[] buttonsTransforms = canvasCollectibles.GetComponentsInChildren<Transform>();

		//Iterate through all the children
		foreach (Transform childTransform in buttonsTransforms) {
			// If the button has the collectible name
			if (childTransform.gameObject.name.Contains (colName)) {
				//Get the button number, being the last character
				string buttonNumber = childTransform.gameObject.name.Substring(childTransform.gameObject.name.Length-1);
				int iButtonNumber = int.Parse (buttonNumber);

				//Lit up the image in the UI
				childTransform.GetComponent<UnityEngine.UI.Image> ().sprite = (Sprite)colSheet[2*(iButtonNumber-1) + 1]; 

				//Mark the collectible as lit
				childTransform.GetComponent<ScriptUISetDropDown> ().bIsLit = true;

				//Mark the collectible in the victory array
				colCompleted[iButtonNumber-1] = true;

				//Check if we already won
				for (int i = 0; i < colCompleted.Length; i++) {
					if (!colCompleted[i])
						break;

					if (i == colCompleted.Length - 1)
						ScriptGameManager.PlayerWon ();
				}
			}
		}
	}

	public void CollectibleImageClick(UnityEngine.UI.Button button){
		//Notify the correct button to set the dropdown panel
		button.GetComponent<ScriptUISetDropDown> ().buttonClicked (bDropDownExpanded, imageExpansionSpeed);
	}
}
