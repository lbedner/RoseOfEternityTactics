using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class RadialMenuController : MonoBehaviour {

	private const string RESOURCE_PATH = "Prefabs/UI/RadialMenu/RadialMenu";

	private const string TALENT_ICON = "talent";
	private const string MAGIC_ICON = "magic";

	private readonly Dictionary<int, string> _combatOptions = new Dictionary<int, string>() {
		{
			0, "Prefabs/Icons/RadialMenu/attack"
		},
		{
			1, "Prefabs/Icons/RadialMenu/defend"
		},
		{
			2, "Prefabs/Icons/RadialMenu/"
		},
		{
			3, "Prefabs/Icons/RadialMenu/item"
		},
		{
			4, "Prefabs/Icons/RadialMenu/cancel"
		}
	};


	public string PopupText { get { return _popupText.text; } set { _popupText.text = value; } }

	public bool IsRootMenuOpen { get; set; }

	[SerializeField] private RadialButtonContainer _radialButtonContainerPrefab;
	[SerializeField] private RadialButtonToolTipController _radialButtonToolTipPrefab;

	private RadialButtonToolTipController _radialButtonToolTipController;

	private Text _popupText;

	private CombatController _combatController;
	private Unit _unit;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		print("RadialMenuController.Awake");
		_combatController = GameObject.Find ("CombatController").GetComponent<CombatController> ();
		_unit = _combatController.HighlightedUnit;
		_popupText = GetComponentInChildren<Text> ();
		_radialButtonToolTipController = InstantiateRadialButtonToolTipController ();
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
				string buttonName = radialButtonType.ToString ();
				int abilityIndex = combatOption.Key;
				string iconPath = combatOption.Value;

				//TODO: Remove this silly hack for abilities
				if (combatOption.Key == 2) {
					string abilityIcon = "";
					string abilityIconPreffix = combatOption.Value;

					if (_unit.UnitData.AbilityCollection.HasTalent) {
						abilityIcon = TALENT_ICON;
						buttonName = "Talent";
					}
					else if (_unit.UnitData.AbilityCollection.HasMagic) {
						abilityIcon = MAGIC_ICON;
						buttonName = "Magic";
					}

					abilityIcon = Path.Combine (abilityIconPreffix, abilityIcon);
					iconPath = abilityIcon;
				}
				RadialButtonController button = InstantiateButton (radialButtonContainer, _combatOptions.Count, abilityIndex, iconPath, radialButtonType, buttonName);

				//TODO: Another stupid hack to classify attacks as abilities
				if (combatOption.Key == 0)
					button.Ability = _unit.UnitData.AbilityCollection.GetAttackAbility ();

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
			button.Item = item;
			radialButtonContainer.Add (button);
		}
		RadialButtonController cancelButton = InstantiateCancelButton (radialButtonContainer, numberOfButtons, items.Count, _combatOptions [4]);
		radialButtonContainer.Add (cancelButton);

		SetRadialButtonContainer (radialButtonContainer);

		IsRootMenuOpen = false;
	}

	/// <summary>
	/// Instantiates the ability buttons.
	/// </summary>
	public void InstantiateAbilityButtons() {

		if (_combatController.CurrentRadialButtonContainer != null) {
			_popupText.text = "";
			StartCoroutine(_combatController.CurrentRadialButtonContainer.ScaleButtonsIn ());
		}

		RadialButtonContainer radialButtonContainer = InstantiateRadialButtonContainer ();

		// Get abilities based off of type
		var abilities = new List<Ability> ();
		UnitData unitData = _unit.UnitData;
		if (unitData.AbilityCollection.HasTalent)
			abilities = unitData.AbilityCollection.GetTalentAbilities ();
		else if (unitData.AbilityCollection.HasMagic) 
			abilities = unitData.AbilityCollection.GetMagicAbilities ();
		
		int numberOfButtons = abilities.Count + 1;
		for (int index = 0; index < abilities.Count; index++) {
			Ability ability = abilities [index];
			RadialButtonController button = InstantiateButton (radialButtonContainer, numberOfButtons, index, ability.IconPath, RadialButtonController.RadialButtonType.ABILITY_USE, ability.Name);
			button.Ability = ability;
			radialButtonContainer.Add (button);
		}
		RadialButtonController cancelButton = InstantiateCancelButton (radialButtonContainer, numberOfButtons, abilities.Count, _combatOptions [4]);
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

		Vector3 position = new Vector3 (x, y, 0.0f) * 90.0f;
		position.z = -1.0f;
		return position;
	}

	/// <summary>
	/// Activates the radial button tool tip.
	/// </summary>
	/// <param name="ability">Ability.</param>
	public void ActivateRadialButtonToolTip(Ability ability) {
		_radialButtonToolTipController.Activate (ability);
	}

	/// <summary>
	/// Deactivates the radial button tool tip.
	/// </summary>
	public void DeactivateRadialButtonToolTip() {
		_radialButtonToolTipController.Deactivate ();
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
		RadialButtonContainer instance = Instantiate(_radialButtonContainerPrefab);
		instance.Initialize (this);
		return instance;
	}

	/// <summary>
	/// Instantiates the radial button tool tip controller.
	/// </summary>
	/// <returns>The radial button tool tip controller.</returns>
	private RadialButtonToolTipController InstantiateRadialButtonToolTipController() {
		RadialButtonToolTipController instance = Instantiate (_radialButtonToolTipPrefab);
		instance.Initialize(this);
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