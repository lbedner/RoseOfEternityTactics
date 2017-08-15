using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnOrderController : MonoBehaviour {

	public Image imagePrefab;

	[SerializeField] private Sprite _statusImage;
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
		foreach (Ally unit in units)
			AddUnit (unit);
	}

	/// <summary>
	/// Adds the unit to the GUI and underlying data structure.
	/// If the unit had been previously persistently highlighted, apply it when added back.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Ordering Index.</param>
	public void AddUnit(Unit unit, int orderIndex = -1) {

		// Instantiate a new Image game object
		Image unitImage = Instantiate (imagePrefab);
		unitImage.sprite = unit.GetPortrait();
		unitImage.transform.SetParent (_panel.transform, false);
		unitImage.name = unit.GetFullName ();

		// Associate unit with image
		TurnOrderImage turnOrderImage = unitImage.GetComponent<TurnOrderImage> ();
		turnOrderImage.Unit = unit;

		// Add image to list
		_images.Add (unit, turnOrderImage);

		// Add unit to turn order list
		if (orderIndex <= -1)
			_turnOrder.AddUnit (unit);
		else {
			_turnOrder.InsertUnit (unit, orderIndex);
			if (unit.HasDeferredAbility)
				_images [unit].ActivateStatusImage (_statusImage);
		}

		// Handle highlighting
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
		_turnOrder.RemoveUnit (unit);
	}

	/// <summary>
	/// Reinserts the unit at the specified index.
	/// If index == -1, unit will be added at the end.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Order index.</param>
	public void ReinsertUnit(Unit unit, int orderIndex = -1) {
		RemoveUnit (unit);
		AddUnit(unit, orderIndex);
		if (orderIndex > -1)
			ReorderIndexes ();
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
	/// If index == -1, unit will be added at the end.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Order index.</param>
	public void FinishTurn(Unit unit, int orderIndex = -1) {
		_highlightOnSpawn = unit.TileHighlighter.IsPersistent;
		ReinsertUnit (unit, orderIndex);
	}

	/// <summary>
	/// Gets all units.
	/// </summary>
	/// <returns>The all units.</returns>
	public List<Unit> GetAllUnits() {
		return _turnOrder.GetAllUnits ();
	}

	/// <summary>
	/// Highlights the unit image with a "selected" color.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void HighlightUnitImage(Unit unit) {
		TurnOrderImage turnOrderImage = _images[unit];
		turnOrderImage.Highlight (false);
		_highlightedImage = turnOrderImage;
	}

	/// <summary>
	/// Highlights the unit image with a "targeted" color.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void TargetUnitImage(Unit unit) {
		TurnOrderImage turnOrderImage = _images[unit];
		turnOrderImage.SetTargeted ();
		_targetedImages.Add (turnOrderImage);
	}

	/// <summary>
	/// De-highlight unit image.
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

	/// <summary>
	/// Reorders the indexes for display purposes.
	/// </summary>
	private void ReorderIndexes() {
		List<Unit> units = _turnOrder.GetAllUnits ();
		for (int index = 0; index < units.Count; index++)
			_images [units [index]].transform.SetSiblingIndex (index);
	}
}