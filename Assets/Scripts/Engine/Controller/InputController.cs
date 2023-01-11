using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputController : MonoBehaviour {

	public const int MOUSE_BUTTON_LEFT = 0;

	public static event EventHandler<InfoEventArgs<Vector3>> mouseMoveEvent;
	public static event EventHandler<InfoEventArgs<int>> selectEvent;

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
	}

	public void OnSelect(InputAction.CallbackContext obj)
	{
		if (obj.performed)
		{
			if (selectEvent != null)
				selectEvent (this, new InfoEventArgs<int> (MOUSE_BUTTON_LEFT));
		}
	}

	public void OnOpenInventory(InputAction.CallbackContext obj)
	{
		if (obj.performed)
		{
			if (keyDownInventoryEvent != null)
				keyDownInventoryEvent (this, new InfoEventArgs<KeyCode> (KeyCode.I));
		}
	}

	public void OnEscape(InputAction.CallbackContext obj)
	{
		if (obj.performed)
		{
			if (keyDownEscapeEvent != null)
				keyDownEscapeEvent (this, new InfoEventArgs<KeyCode> (KeyCode.Escape));
		}
	}
}