using UnityEngine;
using System;

public class InputController : MonoBehaviour {

	public const int MOUSE_BUTTON_LEFT = 0;

	public static event EventHandler<InfoEventArgs<Vector3>> mouseMoveEvent;
	public static event EventHandler<InfoEventArgs<int>> mouseButtonLeftEvent;

	public static event EventHandler<InfoEventArgs<KeyCode>> keyDownInventoryEvent;
	public static event EventHandler<InfoEventArgs<KeyCode>> keyDownEscapeEvent;
	
	private Vector3 _oldMousePosition = Vector3.zero;

	// Update is called once per frame
	void Update () {

		// Check mouse movement
		Vector3 mousePosition = Input.mousePosition;
		if (mousePosition != _oldMousePosition) {
			_oldMousePosition = mousePosition;
			if (mouseMoveEvent != null)
				mouseMoveEvent (this, new InfoEventArgs<Vector3> (mousePosition));
		}

		// Check mouse clicks
		if ((Input.GetMouseButtonDown(MOUSE_BUTTON_LEFT))) {
			if (mouseButtonLeftEvent != null)
				mouseButtonLeftEvent (this, new InfoEventArgs<int> (MOUSE_BUTTON_LEFT));
		}

		// Check key button presses
		if (Input.GetKeyDown (KeyCode.I)) {
			if (keyDownInventoryEvent != null)
				keyDownInventoryEvent (this, new InfoEventArgs<KeyCode> (KeyCode.I));
		}
		else if (Input.GetKeyDown (KeyCode.Escape)) {
			if (keyDownEscapeEvent != null)
				keyDownEscapeEvent (this, new InfoEventArgs<KeyCode> (KeyCode.Escape));
		}				
	}
}