using UnityEngine;
using System.Collections.Generic;

public class UnitMenuController : MonoBehaviour {
	
	[SerializeField] private CharacterSheetController _characterSheetController;
	[SerializeField] private UnitMenuAttributesController _unitMenuAttributesController;
	[SerializeField] private EquippedItemsController _equippedItemsController;

	[SerializeField] private AudioSource _unitSwitchSFX;

	private List<Unit> _units;
	private int _unitIndex = 0;

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize() {
		_units = GameManager.Instance.GetAllies ();
	}

	/// <summary>
	/// Activate the unit menu.
	/// </summary>
	public void Activate() {
		Activate (_units [_unitIndex]);
	}

	/// <summary>
	/// Activate the unit menu.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void Activate(Unit unit) {
		if (!gameObject.activeSelf) {
			UpdateControllers (unit);
			UpdateIndex (unit);
			gameObject.SetActive (true);
		}
	}

	/// <summary>
	/// Deactivate the unit menu.
	/// </summary>
	public void Deactivate() {
		if (gameObject.activeSelf)
			gameObject.SetActive(false);
	}

	/// <summary>
	/// Gets the next unit.
	/// </summary>
	public void NextUnit() {
		_unitIndex++;
		if (_unitIndex > _units.Count - 1)
			_unitIndex = 0;
		UpdateControllers (_units[_unitIndex]);
	}

	/// <summary>
	/// Gets the previous unit.
	/// </summary>
	public void PreviousUnit() {
		_unitIndex--;
		if (_unitIndex < 0)
			_unitIndex = _units.Count - 1;
		UpdateControllers (_units[_unitIndex]);
	}

	/// <summary>
	/// Updates the underlying controllers.
	/// </summary>
	private void UpdateControllers(Unit unit) {
		_characterSheetController.Activate (unit);
		_unitMenuAttributesController.SetAttributes (unit);
		_equippedItemsController.SetEquippedItems (unit);

		_unitSwitchSFX.PlayOneShot (_unitSwitchSFX.clip);
	}

	/// <summary>
	/// Updates the unit list index.
	/// </summary>
	/// <param name="unit">Unit.</param>
	private void UpdateIndex(Unit unit) {
		_unitIndex = _units.IndexOf (unit);
	}
}