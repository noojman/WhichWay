using UnityEngine;
using System.Collections;

public class InstructionsBehavior : MonoBehaviour {

	public static int currentSpriteNum;
	public static bool fadeOut;
	public static bool change;
	bool startAnimation;
	float t;

	public Sprite instructions1;
	public Sprite instructions2;
	public Sprite instructions3;
	public Sprite instructions4;

	SpriteRenderer myRenderer;

	bool beginning;
	
	public static int stage;
	/* 1 : fading in
	 * 2 : fading out
	 */

	// Use this for initialization
	void Start () {
		myRenderer = this.GetComponent<SpriteRenderer> ();
		myRenderer.sprite = null;
		fadeOut = false;
		beginning = true;
		myRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		currentSpriteNum = 1;
		stage = 1;
		startAnimation = false;
		change = true;
		t = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeOut == false) {
			if (change == true) {
				startAnimation = true;
				stage = 1;
				change = false;
			}
			if (startAnimation == true && stage == 1) {
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 1) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.3f;
					myRenderer.color = Color.Lerp (new Color (1.0f, 1.0f, 1.0f, 1.0f), new Color (1.0f, 1.0f, 1.0f, 0.0f), t);
				} else {
					t = 0.0f;
					startAnimation = true;
					stage++;
				}
			}
			if (startAnimation == true && stage == 2) {
				startAnimation = false;
			} else if (startAnimation == false && stage == 2) {
				if (currentSpriteNum == 1) {
					myRenderer.sprite = instructions1;
				} else if (currentSpriteNum == 2) {
					myRenderer.sprite = instructions2;
				} else if (currentSpriteNum == 3) {
					myRenderer.sprite = instructions3;
				} else if (currentSpriteNum == 4) {
					myRenderer.sprite = instructions4;
				}
				startAnimation = true;
				stage++;
			}
			if (startAnimation == true && stage == 3) {
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 3) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.3f;
					myRenderer.color = Color.Lerp (new Color (1.0f, 1.0f, 1.0f, 0.0f), new Color (1.0f, 1.0f, 1.0f, 1.0f), t);
				} else {
					t = 0.0f;
					startAnimation = true;
					stage = 0;
				}
			}
		} else {
			if (startAnimation == true && stage == 1) {
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 1) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.3f;
					myRenderer.color = Color.Lerp (new Color (1.0f, 1.0f, 1.0f, 1.0f), new Color (1.0f, 1.0f, 1.0f, 0.0f), t);
				} else {
					t = 0.0f;
					startAnimation = true;
					stage = 2;
					myRenderer.sprite = instructions4;
					Debug.Log ("Changed to finished sprite");
				}
			}
			if (startAnimation == true && stage == 2) {
				Debug.Log ("Fading out.");
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 2) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.3f;
					myRenderer.color = Color.Lerp (new Color (1.0f, 1.0f, 1.0f, 0.0f), new Color (1.0f, 1.0f, 1.0f, 1.0f), t);
				} else {
					t = 0.0f;
					startAnimation = true;
					stage = 3;
				}
			}
			if (startAnimation == true && stage == 3) {
				Debug.Log ("Fading in.");
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 3) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.7f;
				} else {
					t = 0.0f;
					startAnimation = true;
					stage = 4;
				}
			}
			if (startAnimation == true && stage == 4) {
				Debug.Log ("Fading out.");
				t = 0.0f;
				myRenderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				startAnimation = false;
			} else if (startAnimation == false && stage == 4) {
				if (t < 1.0f) {
					t += Time.deltaTime / 0.3f;
					myRenderer.color = Color.Lerp (new Color (1.0f, 1.0f, 1.0f, 1.0f), new Color (1.0f, 1.0f, 1.0f, 0.0f), t);
				} else {
					t = 0.0f;
					startAnimation = true;
					stage = 0;
					fadeOut = false;
					myRenderer.sprite = null;
				}
			}
		}
	}
}
