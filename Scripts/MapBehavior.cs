using UnityEngine;
using System.Collections;

public class MapBehavior : MonoBehaviour {

	public float ZoomSpeed = 0.1f;

	Transform toDrag;

	private bool dragging;
	private bool wasZooming;

	private Vector3 offset;

	void Start () {
		Input.multiTouchEnabled = true;
		dragging = false;
		wasZooming = false;
	}

	void Update () {
		if (UserBehavior.calibrate == false) {
			if (Input.touchCount == 2) {
				wasZooming = true;

				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				
				if (UserBehavior.setLocation == false) {
					Camera.main.orthographicSize += deltaMagnitudeDiff * ZoomSpeed;
					Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 3.0f);
					//GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * ZoomSpeed;
					//GetComponent<Camera>().orthographicSize = Mathf.Max (Camera.main.orthographicSize, 3.0f);
				}
			}

			Vector3 pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5.0f);
			RaycastHit2D hitInfo = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (pos), Vector2.zero);
			if (wasZooming == true || Input.GetMouseButtonDown(0)) {
				if (Input.touchCount == 1) {
					toDrag = hitInfo.transform;
					pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5.0f);
					pos = Camera.main.ScreenToWorldPoint(pos);
					//pos = GetComponent<Camera>().ScreenToWorldPoint(pos);
					offset = toDrag.position - pos;
					dragging = true;
					wasZooming = false;
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				dragging = false;
				if (Input.touchCount == 1) {
					wasZooming = true;
				}
			}

			if (Input.GetMouseButton(0)) {
				if (Input.touchCount == 1) {
					if (dragging == true && wasZooming == false) {
						pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5.0f);
						pos = Camera.main.ScreenToWorldPoint(pos);
						//pos = GetComponent<Camera>().ScreenToWorldPoint(pos);
						toDrag.position = pos + offset;
					}
				}
			}
		}
	}
}
