using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMovement : MonoBehaviour {
	public float cameraSpeed = 10.0F;
	public float zoomIncrement = 20.0F;	

	public float mobileZoomAdjustment = 1 / 100f;
	public float mobileRotationAdjustment = 1 / 4f;

	public int maxZoom = 80;
	public int minZoom = 30;

	public int rotateSpeed = 100;

	public float jumpSpeed = 700f;

	private Camera cameraRef;
	private Rigidbody playerRB;

	void Start () {
		cameraRef = ScriptGameManager.playerCameraRef;
		playerRB = gameObject.GetComponent<Rigidbody> ();
	}

	void Update () {
		if (!ScriptGameManager.bStuck) {
			if (Input.touchCount > 0) {
				if (Input.touchCount == 1) {
					// Store the touch.
					Touch touchZero = Input.GetTouch (0);

					// If no taps then rotate the camera
					if (touchZero.tapCount == 1) {
						// Find the position in the previous frame of the touch.
						Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;

						// Find the magnitude of the vector (the distance) between the touches in each frame.
						float touchZeroMagnitude = (touchZero.position - touchZeroPrevPos).magnitude;

						// Find the magnitude and direction of the swipe
						float rot = touchZeroMagnitude * rotateSpeed * Time.deltaTime * mobileRotationAdjustment;
						if ((touchZero.position.x - touchZeroPrevPos.x) < 0)
							rot *= -1;
					
						// Rotate the camera
						transform.Rotate (new Vector3 (0, rot, 0));

						// If one tap then move the cube
					} else if (touchZero.tapCount == 2) {
						Vector3 rayWorldPoint = ScriptMouseTouch.worldPoint;
						// If the player is not there yet
						if (!Mathf.Approximately (gameObject.transform.position.magnitude, rayWorldPoint.magnitude)) { 
							// Disconsider the height
							Vector3 auxLoc = rayWorldPoint;
							auxLoc.y = gameObject.transform.position.y;
							// Lerp the position until the player is there taking into consideration its speed
							gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, rayWorldPoint, 1 / (cameraSpeed * Vector3.Distance (gameObject.transform.position, rayWorldPoint)));
					}
					}
				} else if (Input.touchCount == 2) {
					// Store both touches.
					Touch touchZero = Input.GetTouch (0);
					Touch touchOne = Input.GetTouch (1);

					// Find the position in the previous frame of each touch.
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

					// Find the magnitude of the vector (the distance) between the touches in each frame.
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

					// Find the difference in the distances between each frame.
					float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

					// Otherwise change the field of view based on the change in distance between the touches.
					cameraRef.fieldOfView += deltaMagnitudeDiff * zoomIncrement * mobileZoomAdjustment;

					// Clamp the field of view to make sure it's between 0 and 180.
					cameraRef.fieldOfView = Mathf.Clamp (cameraRef.fieldOfView, minZoom, maxZoom);
				}					
			} else {
				float x = cameraSpeed * Input.GetAxis ("Horizontal") * Time.deltaTime;
				float z = cameraSpeed * Input.GetAxis ("Vertical") * Time.deltaTime;

				float rot = 0;
				if (Input.GetKey (KeyCode.Q))
					rot = rotateSpeed * Time.deltaTime;

				if (Input.GetKey (KeyCode.E))
					rot = -rotateSpeed * Time.deltaTime;

				transform.Translate (x, 0, z);
				transform.Rotate (new Vector3 (0, rot, 0));

				float mouseWheel = -Input.GetAxis ("Mouse ScrollWheel");
				if (mouseWheel != 0)
					cameraRef.fieldOfView = Mathf.Clamp (cameraRef.fieldOfView + mouseWheel * zoomIncrement, minZoom, maxZoom);
			}
		}

		if (ScriptGameManager.bVictory) {
			float height = playerRB.transform.position.y;
			if ((playerRB.velocity.magnitude <= 2f) && (height <= 1.2f))
				playerRB.AddForce (new Vector3 (0,jumpSpeed,0));
		}
	}
}