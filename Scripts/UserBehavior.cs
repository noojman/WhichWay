using UnityEngine;
using System.Collections;

public class UserBehavior : MonoBehaviour {
	
	public Texture calibrateButtonTexture;
	public Texture saveButtonTexture;
	public Texture mainMenuButtonTexture;
	public Texture setLocationButtonTexture;
	public Texture undoMoveButtonTexture;
	public Texture clearPathButtonTexture;

	bool fading;

	public static bool calibrate;
	public static bool setLocation;

	public static int stage;

	Vector3 startPosition;
	Vector3 endPosition;

	float t;
	
	private ArrayList pathArray = new ArrayList();

	float loLim = 0.005f; // level to fall to the low state
	float hiLim = 0.1f; // level to go to high state (and detect step)
	int steps = 0; // step counter - counts when comp state goes high
	private bool stateH = false; // comparator state
	
	float fHigh = 10.0f; // noise filter control - reduces frequencies above fHigh
	private float curAcc = 0.0f; // noise filter
	float fLow = 0.1f; // average gravity filter control - time constant about 1/fLow
	private float avgAcc; // average gravity filter

	float speed;

	string dir;

	public static Font myFont;
	
	public static GUIStyle stepStyle = null;

	Compass comp = new Compass();

	bool dragging;
	
	Vector3 direction;

	float myDirection;

	// Use this for initialization
	void Start () {
		myDirection = 0.0f;

		startPosition = new Vector3 (0.0f, 0.0f, 10.0f);
		endPosition = new Vector3 (0.0f, 0.0f, 10.0f);
		direction = new Vector3 (0.0f, 0.0f, 10.0f);

		fading = true;
		dragging = false;
		setLocation = false;
		calibrate = true;

		stage = 1;

		t = 0.0f;
		
		avgAcc = Input.acceleration.magnitude; // initialize avg filter

		comp.enabled = true;

		myFont = Resources.Load ("futura-normal", typeof(Font)) as Font;
		
		stepStyle = new GUIStyle ();
		if (Screen.width <= 640 && Screen.height <= 960) {
			stepStyle.fontSize = 40;
		} else {
			stepStyle.fontSize = 60;
		}
		stepStyle.font = myFont;
		stepStyle.normal.textColor = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
		if (comp.magneticHeading == 0) {
			dir = "N";
		} else if (comp.magneticHeading < 90) {
			dir = "NE";
		} else if (comp.magneticHeading == 90) {
			dir = "E";
		} else if (comp.magneticHeading < 180) {
			dir = "SE";
		} else if (comp.magneticHeading == 180) {
			dir = "S";
		} else if (comp.magneticHeading < 270) {
			dir = "SW";
		} else if (comp.magneticHeading == 270) {
			dir = "W";
		} else {
			dir = "NW";
		}

		if (setLocation == true && Input.GetMouseButtonDown (0)) {
			Debug.Log ("Set Location!");
			Vector3 pos = Input.mousePosition;
			pos.z = 10.0f;
			pos = Camera.main.ScreenToWorldPoint(pos);
			Debug.Log ("New Position: " + pos);
			transform.position = pos;
			setLocation = false;
		}

		if (calibrate == true && Input.GetMouseButtonDown(0)) {
			if (stage == 1) {
				speed = 0.0f;
				Vector3 pos = Input.mousePosition;
				pos.z = 10.0f;
				pos = Camera.main.ScreenToWorldPoint(pos);
				Debug.Log ("Starting Position: " + pos);
				transform.position = pos;
				startPosition = pos;
				stage++;
				InstructionsBehavior.change = true;
				InstructionsBehavior.currentSpriteNum = 2;

			} else if (stage == 2) {
				// filter input.acceleration using Lerp
				curAcc = Mathf.Lerp (curAcc, Input.acceleration.magnitude, fHigh);
				avgAcc = Mathf.Lerp (avgAcc, Input.acceleration.magnitude, fLow);
				float delta = curAcc - avgAcc; // gets the acceleration pulses
				
				if (stateH == false) { // if state == false...
					if (delta > hiLim) { // only goes high if input > hiLim
						steps++;
						stateH = true;
					}
				} else {
					if (delta < loLim) { // only goes low if input < loLim
						stateH = false;
					}
				}
				Vector3 pos = Input.mousePosition;
				pos.z = 10.0f;
				pos = Camera.main.ScreenToWorldPoint(pos);
				Debug.Log ("Ending Position: " + pos);
				transform.position = pos;
				endPosition = pos;

				//Vector2 temp = new Vector2 (startPosition.x - endPosition.x, startPosition.y - endPosition.y);
				//speed = 50.0f * temp.magnitude / steps;

				speed = 45.0f;

				GameObject newPath = Resources.Load ("Path") as GameObject;
				GameObject newPathClone = Instantiate (newPath, startPosition, this.gameObject.transform.rotation) as GameObject;
				newPathClone.transform.parent = GameObject.Find("Background").transform;
				pathArray.Add(newPathClone);

				stage++;
				InstructionsBehavior.change = true;
				InstructionsBehavior.currentSpriteNum = 3;
			} else if (stage == 3) {
				if (Input.GetMouseButtonDown (0)) {
					Debug.Log ("Calibration mouse down.");
					startPosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f);
				}
				if (Input.GetMouseButton (0)) {
					dragging = true;
				}
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			if (dragging == true) {
				Debug.Log ("Calibration mouse up.");
				endPosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f);
				direction = endPosition - startPosition;

				if (direction.x > 0.0f && direction.y < 0.0f) {
					myDirection = 90.0f + (Mathf.Atan2(Mathf.Abs(direction.y), direction.x) * Mathf.Rad2Deg);
				} else if (direction.x < 0.0f && direction.y < 0.0f) {
					myDirection = 270.0f - (Mathf.Atan2(Mathf.Abs(direction.y), Mathf.Abs(direction.x)) * Mathf.Rad2Deg);
				} else if (direction.x < 0.0f && direction.y > 0.0f) {
					myDirection = 270.0f + (Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg);
				} else {
					myDirection = 90.0f - (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
				}
				
				Debug.Log("Direction: " + myDirection);

				myDirection = comp.magneticHeading - myDirection;
				
				stage = 0;
				steps = 0;
				InstructionsBehavior.fadeOut = true;
				InstructionsBehavior.stage = 1;
				dragging = false;
				stateH = false;
				calibrate = false;
				setLocation = false;
			}
		}
	}
	
	void FixedUpdate () {
		if (setLocation == true || calibrate == true) {
			if (t < 1.0f && fading == true) {
				t += Time.deltaTime / 0.3f;
				GetComponent<Renderer>().material.color = Color.Lerp (new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), t);
			} else if (t > 1.0f && fading == true) {
				t = 0.0f;
				fading = false;
			} else if (t < 1.0f && fading == false) {
				t += Time.deltaTime / 0.3f;
				GetComponent<Renderer>().material.color = Color.Lerp (new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), t);
			} else if (t > 1.0f && fading == false) {
				t = 0.0f;
				fading = true;
			}
		} else if (setLocation == false && calibrate == false) {
			GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			// Orient an object to point northward.
			//transform.rotation = Quaternion.Euler(0, 0, -comp.magneticHeading);
			transform.rotation = Quaternion.AngleAxis (myDirection - comp.magneticHeading, Vector3.forward);

			// filter input.acceleration using Lerp
			curAcc = Mathf.Lerp (curAcc, Input.acceleration.magnitude, fHigh);
			avgAcc = Mathf.Lerp (avgAcc, Input.acceleration.magnitude, fLow);
			float delta = curAcc - avgAcc; // gets the acceleration pulses

			if (stateH == false) { // if state == false...
				if (delta > hiLim) { // only goes high if input > hiLim
					if (steps % 2 == 0) {
						GameObject newPath = Resources.Load ("Path") as GameObject;
						GameObject newPathClone = Instantiate (newPath, new Vector3 (transform.position.x, 
                                         transform.position.y, 10.0f), this.gameObject.transform.rotation) as GameObject;
						newPathClone.transform.parent = GameObject.Find("Background").transform;
						pathArray.Add(newPathClone);
					}
					steps++;
					stateH = true;
					this.gameObject.transform.position += this.gameObject.transform.up * speed * Time.deltaTime;
				}
			} else {
				if (delta < loLim) { // only goes low if input < loLim
					stateH = false;
				}
			}
		}
	}
	
	void OnGUI() {
		if (GUI.Button (new Rect (0, 0, Screen.width / 3, Screen.width / 8), calibrateButtonTexture)) {
			Debug.Log("Clicked calibrate button.");
			if (calibrate == false) {
				stage = 1;
				InstructionsBehavior.change = true;
				InstructionsBehavior.currentSpriteNum = 1;
				calibrate = true;
			}
		}

		if (GUI.Button (new Rect (Screen.width / 3, 0, Screen.width / 3, Screen.width / 8), saveButtonTexture)) {
			Debug.Log("Clicked save button.");

		}
		
		if (GUI.Button (new Rect (2 * Screen.width / 3, 0, Screen.width / 3, Screen.width / 8), mainMenuButtonTexture)) {
			Debug.Log("Clicked main menu button.");
			if (setLocation == false) {
				//Application.LoadLevel(0);
			}
		}

		if (GUI.Button (new Rect (0, Screen.height - (Screen.width / 9), Screen.width / 3, Screen.width / 8), setLocationButtonTexture)) {
			Debug.Log("Clicked set Location button.");
			if (setLocation == false) {
				setLocation = true;
			} else {
				setLocation = false;
			}
		}

		if (GUI.Button (new Rect (Screen.width / 3, Screen.height - (Screen.width / 9), Screen.width / 3, Screen.width / 8), undoMoveButtonTexture)) {
			Debug.Log("Clicked undo move button.");
			if (setLocation == false) {
				if (pathArray.Count > 0) {
					int index = pathArray.Count - 1;
					GameObject tempPath = pathArray[index] as GameObject;
					transform.position = tempPath.transform.position;
					transform.rotation = tempPath.transform.rotation;
					Destroy (tempPath);
					pathArray.RemoveAt(index);
					if (steps % 2 != 0) {
						steps--;
					} else {
						steps -= 2;
					}
				}
			}
		}

		if (GUI.Button (new Rect (2 * Screen.width / 3, Screen.height - (Screen.width / 9), Screen.width / 3, Screen.width / 8), clearPathButtonTexture)) {
			Debug.Log("Clicked clear path button.");
			if (setLocation == false) {
				GameObject[] PathArray;
				PathArray = GameObject.FindGameObjectsWithTag("Path");
				foreach(GameObject i in PathArray) {
					Destroy(i);
				}
			}
		}

		/*
		string text1 = "STEPS: " + steps + " | " + dir;
		if (Screen.width <= 640 && Screen.height <= 960) {
			// screen compatibility
			GUI.Label (new Rect(Screen.width / 3, Screen.height - 42, 70, 70), text1, stepStyle);
		} else {
			GUI.Label (new Rect(Screen.width / 3, Screen.height - 60, 100, 100), text1, stepStyle);
		}
		*/
	}
}
