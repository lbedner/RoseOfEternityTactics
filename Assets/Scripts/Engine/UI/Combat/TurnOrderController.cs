﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnOrderController : MonoBehaviour {

	public Image imagePrefab;

	[SerializeField] private TurnOrderUnitStatus _turnOrderUnitStatus;

	private GameObject _panel;

	private Dictionary<Unit, TurnOrderImage> _images = new Dictionary<Unit, TurnOrderImage>();

	private TurnOrder _turnOrder;

	private TurnOrderImage _highlightedImage = null;
	private List<TurnOrderImage> _targetedImages = new List<TurnOrderImage> ();

	private bool _highlightOnSpawn = false;

	public bool IsImageHighlighted { get; set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		print ("TurnOrderController.Awake()");
		_turnOrder = new TurnOrder ();
		_panel = this.gameObject;
	}		

	/// <summary>
	/// Adds the units to the GUI and underlying data structure.
	/// </summary>
	/// <param name="units">Units.</param>
	public void AddUnits(List<Unit> units) {
		foreach (Ally unit in units) {
			AddUnit (unit);
		}
	}

	/// <summary>
	/// Adds the unit to the GUI and underlying data structure.
	/// If the unit had been previously persistently highlighted, apply it when added back.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void AddUnit(Unit unit) {
		Image unitImage = Instantiate (imagePrefab);
		unitImage.sprite = unit.GetPortrait();
		unitImage.transform.SetParent (_panel.transform, false);

		TurnOrderImage turnOrderImage = unitImage.GetComponent<TurnOrderImage> ();
		turnOrderImage.Unit = unit;

		_images.Add (unit, turnOrderImage);
		_turnOrder.AddCombatant (unit);

		if (_highlightOnSpawn)
			turnOrderImage.Highlight (false);
		_highlightOnSpawn = false;
	}

	/// <summary>
	/// Removes the unit from the GUI and the underlying data structure.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void RemoveUnit(Unit unit) {
		TurnOrderImage turnOrderImage = _images[unit];
		turnOrderImage.transform.SetParent (null);		

		_targetedImages.Remove (_images[unit]);
		Destroy (_images [unit]);
		_images.Remove (unit);
		_turnOrder.RemoveCombatant (unit);
	}

	/// <summary>
	/// Gets the unit that is next up.
	/// </summary>
	/// <returns>The next up.</returns>
	public Unit GetNextUp() {
		return _turnOrder.GetNextUp ();
	}

	/// <summary>
	/// Finishs the turn and moves the unit on the GUI and the underlying data structure.
	/// If the unit had been previously persistently highlighted, add flag so it's reapplied.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void FinishTurn(Unit unit) {
		_highlightOnSpawn = unit.TileHighlighter.IsPersistent;
		RemoveUnit (unit);
		AddUnit (unit);
	}

	/// <summary>
	/// Gets all units.
	/// </summary>
	/// <returns>The all units.</returns>
	public List<Unit> GetAllUnits() {
		return _turnOrder.GetAllUnits ();
	}

	/// <summary>
	/// Highlights the unit image.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void HighlightUnitImage(Unit unit) {
		TurnOrderImage turnOrderImage = _images[unit];
		turnOrderImage.Highlight (false);
		_highlightedImage = turnOrderImage;
	}

	/// <summary>
	/// Highlights the unit image.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void TargetUnitImage(Unit unit) {
		TurnOrderImage turnOrderImage = _images[unit];
		turnOrderImage.SetTargeted ();
		_targetedImages.Add (turnOrderImage);
	}

	/// <summary>
	/// Des the highlight unit image.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void DeHighlightUnitImage() {
		if (_highlightedImage != null)
			_highlightedImage.DeHighlight ();
	}

	/// <summary>
	/// Untargets the unit images.
	/// </summary>
	public void UntargetUnitImages() {
		foreach (var image in _targetedImages)
			if (!image.Unit.TileHighlighter.IsPersistent)
				image.DeHighlight ();
		_targetedImages.Clear ();
	}

	/// <summary>
	/// Activates the turn order unit status.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void ActivateTurnOrderUnitStatus(Unit unit) {
		_turnOrderUnitStatus.Activate (unit);
	}

	/// <summary>
	/// Deactivates the turn order unit status.
	/// </summary>
	public void DeactivateTurnOrderUnitStatus() {
		_turnOrderUnitStatus.Deactivate ();
	}
}