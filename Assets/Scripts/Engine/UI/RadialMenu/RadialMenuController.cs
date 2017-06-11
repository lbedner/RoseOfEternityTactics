﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RadialMenuController : MonoBehaviour {

	private const string RESOURCE_PATH = "Prefabs/UI/RadialMenu/RadialMenu";

	private static Dictionary<int, string> _combatOptions = new Dictionary<int, string>() {
		{
			0, "Prefabs/Icons/RadialMenu/attack"
		},
		{
			1, "Prefabs/Icons/RadialMenu/defend"
		},
		{
			2, "Prefabs/Icons/RadialMenu/magic"
		},
		{
			3, "Prefabs/Icons/RadialMenu/item"
		}
	};

	public RadialButtonContainer radialButtonContainerPrefab;

	public string PopupText { get { return _popupText.text; } set { _popupText.text = value; } }

	public bool IsRootMenuOpen { get; set; }

	private Text _popupText;

	private CombatController _combatController;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		_combatController = GameObject.Find ("CombatController").GetComponent<CombatController> ();
		_popupText = GetComponentInChildren<Text> ();
		InstantiateRootButtons();
	}

	/// <summary>
	/// Instantiate the specified canvas.
	/// </summary>
	/// <param name="canvas">Canvas.</param>
	public static RadialMenuController InstantiateInstance(Canvas canvas) {

		// Create radial menu if doesn't exist already
		RadialMenuController radialMenuController = canvas.GetComponentInChildren<RadialMenuController>(true);
		if (radialMenuController == null) {
			GameObject instance = Instantiate (Resources.Load<GameObject> (RESOURCE_PATH));
			radialMenuController = instance.GetComponent<RadialMenuController> ();
			radialMenuController.transform.SetParent (canvas.transform, false);
		}
		else
			radialMenuController.gameObject.SetActive (true);

		return radialMenuController;
	}

	/// <summary>
	/// Instantiates the buttons.
	/// </summary>
	public void InstantiateRootButtons() {

		RadialButtonContainer radialButtonContainer;

		// If there is already one, use that one
		if (_combatController.CurrentRadialButtonContainer != null) {
			radialButtonContainer = _combatController.CurrentRadialButtonContainer;
			radialButtonContainer.gameObject.SetActive (true);
			StartCoroutine(radialButtonContainer.ScaleButtonsOut ());
		}
		else {			

			radialButtonContainer = InstantiateRadialButtonContainer ();

			foreach (var combatOption in _combatOptions) {
				RadialButtonController.RadialButtonType radialButtonType = (RadialButtonController.RadialButtonType)combatOption.Key;
				RadialButtonController button = InstantiateButton (radialButtonContainer, _combatOptions.Count, combatOption.Key, combatOption.Value, radialButtonType, radialButtonType.ToString ());

				radialButtonContainer.Add (button);
			}
		}

		SetRadialButtonContainer (radialButtonContainer);

		IsRootMenuOpen = true;
	}

	/// <summary>
	/// Instantiates the item buttons.
	/// </summary>
	public void InstantiateItemButtons() {

		if (_combatController.CurrentRadialButtonContainer != null) {
			_popupText.text = "";
			StartCoroutine(_combatController.CurrentRadialButtonContainer.ScaleButtonsIn ());
		}
			
		RadialButtonContainer radialButtonContainer = InstantiateRadialButtonContainer ();
		List<Item> items = ItemManager.Instance.GlobalInventory.GetConsumables ();
		int numberOfButtons = items.Count + 1;
		for (int index = 0; index < items.Count; index++) {
			Item item = items [index];
			RadialButtonController button = InstantiateButton (radialButtonContainer, numberOfButtons, index, item.IconPath, RadialButtonController.RadialButtonType.ITEM_USE, item.Name);
			radialButtonContainer.Add (button);
		}
		RadialButtonController cancelButton = InstantiateCancelButton (radialButtonContainer, numberOfButtons, items.Count, _combatOptions [2]);
		radialButtonContainer.Add (cancelButton);

		SetRadialButtonContainer (radialButtonContainer);

		IsRootMenuOpen = false;
	}

	/// <summary>
	/// Instantiates the last used buttons.
	/// If there are none, start at root.
	/// </summary>
	public void InstantiateLastUsedButtons() {
		int count = _combatController.RadialButtonContainers.Count;
		if (count > 0) {
			StartCoroutine(_combatController.RadialButtonContainers [count - 1].ScaleButtonsOut ());
		}
		else
			InstantiateRootButtons ();
	}

	/// <summary>
	/// Drills up.
	/// </summary>
	public void DrillUp() {
		StartCoroutine (_combatController.CurrentRadialButtonContainer.ScaleButtonsIn ());
		StartCoroutine (_combatController.PreviousRadialButtonContainer.ScaleButtonsOut ());

		_combatController.RadialButtonContainers.Remove (_combatController.CurrentRadialButtonContainer);
		Destroy (_combatController.CurrentRadialButtonContainer.gameObject);

		_combatController.CurrentRadialButtonContainer = _combatController.PreviousRadialButtonContainer;

		if (_combatController.RadialButtonContainers.Count == 1)
			IsRootMenuOpen = true;
	}

	/// <summary>
	/// Gets the button position.
	/// </summary>
	/// <returns>The button position.</returns>
	/// <param name="numberOfButtons">Number of buttons.</param>
	/// <param name="index">Index.</param>
	public Vector3 GetButtonPosition(int numberOfButtons, int index) {
		float theta = (2 * Mathf.PI / numberOfButtons) * index;
		float x = Mathf.Sin (theta);
		float y = Mathf.Cos (theta);

		Vector3 position = new Vector3 (x, y, 0.0f) * 75.0f;
		position.z = -1.0f;
		return position;
	}

	/// <summary>
	/// Instantiates the button.
	/// </summary>
	/// <returns>The button.</returns>
	/// <param name="radialButtonContainer">Radial button container.</param>
	/// <param name="numberOfButtons">Number of buttons.</param>
	/// <param name="index">Index.</param>
	/// <param name="iconPath">Icon path.</param>
	/// <param name="radialButtonType">Radial button type.</param>
	/// <param name="name">Name.</param>
	private RadialButtonController InstantiateButton(
		RadialButtonContainer radialButtonContainer,
		int numberOfButtons,
		int index,
		string iconPath,
		RadialButtonController.RadialButtonType radialButtonType,
		string name) {

		Vector3 buttonPosition = GetButtonPosition (numberOfButtons, index);
		RadialButtonController radialButtonController = RadialButtonController.InstantiateInstance (radialButtonContainer);
		radialButtonController.Initialize (iconPath, radialButtonType, GetFirstLetterToUpper (name), buttonPosition);

		return radialButtonController;
	}

	/// <summary>
	/// Instantiates the cancel button.
	/// </summary>
	/// <returns>The cancel button.</returns>
	/// <param name="container">Container.</param>
	/// <param name="numberOfButtons">Number of buttons.</param>
	/// <param name="index">Index.</param>
	/// <param name="iconPath">Icon path.</param>
	private RadialButtonController InstantiateCancelButton(RadialButtonContainer container, int numberOfButtons, int index, string iconPath) {
		RadialButtonController.RadialButtonType radialButtonType = RadialButtonController.RadialButtonType.CANCEL;
		RadialButtonController cancelButton = InstantiateButton(container, numberOfButtons, index, iconPath, radialButtonType, radialButtonType.ToString());

		return cancelButton;
	}

	/// <summary>
	/// Gets the first letter to upper.
	/// </summary>
	/// <returns>The first letter to upper.</returns>
	/// <param name="s">S.</param>
	private string GetFirstLetterToUpper(string s) {
		StringBuilder sb = new StringBuilder ();
		for (int index = 0; index < s.Length; index++) {
			string character = s.Substring (index, 1);
			if (index == 0 && !Char.IsUpper (character.ToCharArray () [0]))
				character = character.ToUpper ();
			else if (index != 0 && Char.IsUpper (character.ToCharArray () [0]))
				character = character.ToLower ();
			sb.Append (character);
		}
		return sb.ToString ();
	}

	/// <summary>
	/// Instantiates the radial button container.
	/// </summary>
	/// <returns>The radial button container.</returns>
	private RadialButtonContainer InstantiateRadialButtonContainer() {
		RadialButtonContainer instance = Instantiate(radialButtonContainerPrefab);
		instance.Initialize (this);
		return instance;
	}

	/// <summary>
	/// Sets the radial button container.
	/// </summary>
	/// <param name="radialButtonContainer">Radial button container.</param>
	private void SetRadialButtonContainer(RadialButtonContainer radialButtonContainer) {
		_combatController.RadialButtonContainers.Add (radialButtonContainer);
		_combatController.PreviousRadialButtonContainer = _combatController.CurrentRadialButtonContainer;
		_combatController.CurrentRadialButtonContainer = radialButtonContainer;
	}
}