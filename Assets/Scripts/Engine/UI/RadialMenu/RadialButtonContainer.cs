using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadialButtonContainer : MonoBehaviour {

	public RadialMenuController RadialMenuController { get; private set; }

	private List<RadialButtonController> _radialButtonControllers = new List<RadialButtonController>();

	/// <summary>
	/// Initialize the specified radialMenuController.
	/// </summary>
	/// <param name="radialMenuController">Radial menu controller.</param>
	public void Initialize(RadialMenuController radialMenuController) {
		RadialMenuController = radialMenuController;
		transform.SetParent (RadialMenuController.transform, false);
		transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
	}

	/// <summary>
	/// Add the specified radialButtonController.
	/// </summary>
	/// <param name="radialButtonController">Radial button controller.</param>
	public void Add(RadialButtonController radialButtonController) {
		_radialButtonControllers.Add (radialButtonController);
	}

	/// <summary>
	/// Scales the buttons in.
	/// </summary>
	public IEnumerator ScaleButtonsIn() {
		foreach (var item in _radialButtonControllers)
			StartCoroutine (item.ScaleButtonIn ());
		yield return null;
	}

	/// <summary>
	/// Scales the buttons out.
	/// </summary>
	public IEnumerator ScaleButtonsOut() {
		int count = _radialButtonControllers.Count;
		for (int index = 0; index < count; index++) {
			RadialButtonController button = _radialButtonControllers [index];
			Vector3 position = RadialMenuController.GetButtonPosition (count, index);
			StartCoroutine (button.ScaleButtonOut (position));
		}
		yield return null;
	}
}