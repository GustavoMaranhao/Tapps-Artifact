using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptFarmingResources : MonoBehaviour {
	[HideInInspector]
	public float farmWidth;
	[HideInInspector]
	public float farmHeight;
	[HideInInspector]
	public bool bSpawnNext = true;


	public GameObject[] spawnObjects;
	public Color[] spawnColors;

	public float spawnSpeed = 10f;
	public int maxObjectsSpawned = 10;

	private GameObject[] farmObjects;

	private int objectsSpawned = 0;

	void Start () {	
		farmObjects = new GameObject[maxObjectsSpawned];

		//Create the objects and keep them in a pool
		for (int i = 0; i < maxObjectsSpawned; i++) {			
			//There might be no objects of one type or there might be only that type, decided at start
			int indexToPick = Random.Range (0, spawnObjects.Length);
			while (spawnObjects[indexToPick] == null)
				indexToPick = Random.Range (0, spawnObjects.Length);

			farmObjects[i] = Instantiate (spawnObjects[indexToPick], new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			farmObjects[i].gameObject.transform.parent = gameObject.transform;
			farmObjects[i].SetActive (false);
		}
	}

	void Update () {
		if (ScriptGameManager.bStartMinigame && bSpawnNext) {
			//There is still space to spawn objects and making sure we have objects in the array
			if ((objectsSpawned < (maxObjectsSpawned)) && (farmObjects.Length > 0)) {
				//Randomly chooses an object from array
				int indexToPick = Random.Range (0, farmObjects.Length);

				//If that object is active, simply get the next non active one in the array
				if (farmObjects[indexToPick].activeInHierarchy) {
					indexToPick = -1;
					for (int i = 0; i < farmObjects.Length; i++) {
						indexToPick = farmObjects [i].activeInHierarchy ? -1 : i;
						if (indexToPick > -1)
							break;
					}
				}
					
				//Making sure we have a valid index and the object wasnt used yet
				if ((indexToPick > -1) && (farmObjects [indexToPick].GetComponent<ScriptFarmingObject>().bCanPickup)) {
					Color objectColor;

					//Making sure we didnt forget to fill the array
					if (spawnColors.Length == 0)
						objectColor = Color.gray;
					else {
						//Randomly pick a color to use
						int colorIndex = Random.Range (0, spawnColors.Length);
						objectColor = spawnColors [colorIndex];
					}
					farmObjects [indexToPick].GetComponent<Renderer>().material.color = objectColor;

					//Get top left corner of the tile - mesh bounds extents is always half the size of the dimension
					float topX = gameObject.transform.position.x + gameObject.GetComponent<MeshRenderer> ().bounds.extents.x;
					float topZ = gameObject.transform.position.z - gameObject.GetComponent<MeshRenderer> ().bounds.extents.z;

					//Get bottom right corner of the tile - mesh bounds extents is always half the size of the dimension
					float botX = gameObject.transform.position.x - gameObject.GetComponent<MeshRenderer> ().bounds.extents.x;
					float botZ = gameObject.transform.position.z + gameObject.GetComponent<MeshRenderer> ().bounds.extents.z;

					//Assign values to height and width
					farmWidth = Mathf.Abs(topX - botX);
					farmHeight = Mathf.Abs (topZ - botZ);

					//Get objects bounds
					Vector3 objBounds = farmObjects[indexToPick].GetComponent<MeshRenderer> ().bounds.extents;

					//Pick a random location on the tile - mesh bounds extents is always half the size of the dimension
					float x = Random.Range(topX - objBounds.x, botX + objBounds.x);				
					float y = objBounds.y * 2;
					float z = Random.Range(topZ + objBounds.z, botZ - objBounds.z);

					//Set the position and activate
					farmObjects [indexToPick].transform.position = new Vector3 (x, y, z);
					farmObjects [indexToPick].SetActive (true);

					//Call the scaling function in the object
					farmObjects[indexToPick].GetComponent<ScriptFarmingObject>().ChangeScale();

					objectsSpawned++;
					bSpawnNext = false;
				}
			}
		}
	}

	public void DeactivateFarmObjects(){		
		//Deactivate all the objects
		if (farmObjects != null) {
			for (int i = 0; i < farmObjects.Length; i++) {
				farmObjects[i].SetActive(false);
			}

			objectsSpawned = 0;
			bSpawnNext = true;
		}

		//Reshuffle the materials every time we leave
		Start ();
	}
}
