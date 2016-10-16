using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.name.Equals ("Set Location Button") && UserBehavior.setLocation == true) {
			GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		} else {
			GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
