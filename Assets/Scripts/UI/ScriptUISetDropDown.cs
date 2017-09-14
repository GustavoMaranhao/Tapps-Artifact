using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptUISetDropDown : MonoBehaviour, IPointerEnterHandler {
	[HideInInspector]
	public bool bIsLit = false;

	public string tooltipText;
	public string tooltipTextCompleted;

	private GameObject dropDownPanel;
	private bool bExpanded = false;

	void Start () {
		dropDownPanel = ScriptGameManager.ScriptUIRef.dropDownPanel;
	}

	public void OnPointerEnter(PointerEventData eventData){
		//Only do the hover events if the dropdown isnt expanded
		if (!ScriptGameManager.ScriptUIRef.bDropDownExpanded) {
			//On mouse hover we want to change only the x position
			Vector3 newPos = dropDownPanel.GetComponent<RectTransform> ().localPosition;
			newPos.x = eventData.pointerEnter.GetComponent<RectTransform> ().localPosition.x;

			//And set the panels position as a highlight
			dropDownPanel.GetComponent<RectTransform> ().localPosition = newPos;
		}
	}

	public void buttonClicked(bool bIsUIExpanded, float expansionSpeed){
		ScriptUIMoveSprite dropDownMoveScript = dropDownPanel.GetComponent<ScriptUIMoveSprite> ();

		//If we arent moving the panel already
		if (!dropDownMoveScript.bShouldExpand) {
			//The starting position will always be where the panel is
			dropDownMoveScript.startPosition = dropDownPanel.transform.position;

			//If the UI isnt expanded, any can expand
			if (!bIsUIExpanded) {
				//If the collectible is lit in the UI show the completed text, otherwise the normal text
				if (bIsLit)
					dropDownPanel.GetComponentInChildren<UnityEngine.UI.Text> ().text = tooltipTextCompleted;
				else
					dropDownPanel.GetComponentInChildren<UnityEngine.UI.Text> ().text = tooltipText;
				//Show the text
				dropDownPanel.GetComponentInChildren<UnityEngine.UI.Text> ().enabled = true;

				//The final position will be its "height" units below
				float pos = -dropDownPanel.GetComponent<RectTransform>().rect.height; 

				//Set the endPosition keeping the x value of the starting poisition
				dropDownMoveScript.endPosition = dropDownMoveScript.startPosition;
				dropDownMoveScript.endPosition.y += pos;

				//Mark this as the expanded panel
				bExpanded = true;
				//And notify the UI
				ScriptGameManager.ScriptUIRef.bDropDownExpanded = true;

				//Set the speed to expand
				dropDownMoveScript.expandSpeed = expansionSpeed;

				//Tell the panel to start its movement
				dropDownMoveScript.StartJourney();
			} 
			//If the UI is expanded, only the button that expanded it can contract
			else {
				if (bExpanded) {
					//Hide the text
					dropDownPanel.GetComponentInChildren<UnityEngine.UI.Text> ().enabled = false;

					//The end position will be the current button minus its height
					dropDownMoveScript.endPosition = gameObject.GetComponent<RectTransform> ().position;
					dropDownMoveScript.endPosition.y = ScriptGameManager.ScriptUIRef.panelStartHeight;

					//Unmark this as the expanded panel
					bExpanded = false;
					//And notify the UI
					ScriptGameManager.ScriptUIRef.bDropDownExpanded = false;

					//Set the speed to expand
					dropDownMoveScript.expandSpeed = expansionSpeed;

					//Tell the panel to start its movement
					dropDownMoveScript.StartJourney();
				}
			}
		}
	}
}
