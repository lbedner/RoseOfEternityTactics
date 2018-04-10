using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EternalEngine;

public class TurnOrderController : MonoBehaviour {

	private const int CLOCK_TICKS_TILL_COMBAT_READY = 100;

	public Image imagePrefab;

	[SerializeField] private Sprite _statusImage;
	[SerializeField] private TurnOrderUnitStatus _turnOrderUnitStatus;
	[SerializeField] private Image _dividerPrefab;

	private GameObject _panel;

	private TurnOrder _preSortedTurnOrder;

	private TurnOrderCollection _turnOrderCollection;
	private TurnOrderImageCollection _turnOrderImageCollection;

	private List<TurnOrderImage> _highlightedImages = new List<TurnOrderImage>();
	private List<TurnOrderImage> _targetedImages = new List<TurnOrderImage> ();

	private bool _highlightOnSpawn = false;

	private Image _dividerImage;

	public bool IsImageHighlighted { get; set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		print ("TurnOrderController.Awake()");
		_preSortedTurnOrder = new TurnOrder ();
		_turnOrderCollection = new TurnOrderCollection ();
		_turnOrderCollection.Primary = new TurnOrder();
		_turnOrderCollection.Secondary = new TurnOrder ();
		_turnOrderImageCollection = new TurnOrderImageCollection (_turnOrderCollection.Primary, _turnOrderCollection.Secondary);
		_panel = this.gameObject;
		_dividerImage = Instantiate (_dividerPrefab);
	}

	/// <summary>
	/// Adds the unsorted unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void AddUnsortedUnit(Unit unit) {
		_preSortedTurnOrder.AddUnit (unit);
	}

	/// <summary>
	/// Sorts the turn order.
	/// </summary>
	/// <param name="turnOrderCollection">Turn order collection.</param>
	public void SortTurnOrder(TurnOrder turnOrder) {

		_preSortedTurnOrder.Union (turnOrder);

		RemoveAllUnits (turnOrder);
		if (turnOrder == _turnOrderCollection.Primary)
			_dividerImage.transform.SetParent (null);
		
		int index = 1;
		do {
			foreach (var unit in _preSortedTurnOrder.GetAllUnits()) {

				// Increment clocks ticks by speed attribute. 
				// If this bring the unit to combat readiness,
				// add them to queue and set property
				unit.IncrementClockTicks(turnOrder, (int)unit.GetSpeedAttribute ().CurrentValue);
				if (unit.GetClockTicks(turnOrder) >= CLOCK_TICKS_TILL_COMBAT_READY) {

					bool isCombatReady = unit.IsCombatReady(turnOrder);
					int clockTicks = unit.GetClockTicks(turnOrder);
					int roundAppearences = unit.GetRoundAppearences(turnOrder);
				 	if (!isCombatReady || clockTicks >= CLOCK_TICKS_TILL_COMBAT_READY * (roundAppearences + 1)) {
						print(string.Format("{0} - CT:{1} - Index:{2} - RoundAppearences:{3} - IsCombatReady:{4}", unit.GetFullName(), unit.GetClockTicks(turnOrder), index, roundAppearences, isCombatReady));
						unit.SetCombatReady(turnOrder, true);
						unit.IncrementRoundAppearences(turnOrder);
						AddUnit (unit, turnOrder);
					}
				}
			}
			index++;
		} while (!AreAllUnitsCombatReady(_preSortedTurnOrder.GetAllUnits(), turnOrder));

		_preSortedTurnOrder.Clear ();

		// Add divider to separate turn order rounds
		if (turnOrder == _turnOrderCollection.Primary)
			AddDivider ();
	}

	/// <summary>
	/// Adds the unit to the GUI and underlying data structure.
	/// If the unit had been previously persistently highlighted, apply it when added back.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Ordering Index.</param>
	public void AddUnit(Unit unit, TurnOrder turnOrder, int orderIndex = -1) {

		// Instantiate a new Image game object
		Image unitImage = Instantiate (imagePrefab);
		unitImage.sprite = unit.GetPortrait();
		unitImage.transform.SetParent (_panel.transform, false);

		// Associate unit with image
		TurnOrderImage turnOrderImage = unitImage.GetComponent<TurnOrderImage> ();
		turnOrderImage.Unit = unit;

		unitImage.name = string.Format("{0} - {1}", unit.GetFullName (), turnOrderImage.GetInstanceID());

		// Add image to list
		_turnOrderImageCollection.Add(turnOrderImage, turnOrder);

		// Add unit to turn order list
		if (orderIndex <= -1)
			turnOrder.AddUnit (unit);
		else {
			turnOrder.InsertUnit (unit, orderIndex);
			if (unit.HasDeferredAbility)
				_turnOrderImageCollection.Get (unit, turnOrder).ActivateStatusImage (_statusImage);
		}

		// Handle highlighting
		if (_highlightOnSpawn)
			turnOrderImage.Highlight (false);
		_highlightOnSpawn = false;
	}

	/// <summary>
	/// Removes all units.
	/// </summary>
	/// <param name="turnOrder">Turn order.</param>
	public void RemoveAllUnits(TurnOrder turnOrder) {
		List<Unit> units = turnOrder.GetAllUnits ();
		for (int i = units.Count - 1; i >= 0; i--) {
			Unit unit = units [i];
			unit.ResetTurnOrderData (turnOrder);
			RemoveUnit (unit, turnOrder);
		}
	}

	/// <summary>
	/// Removes the unit from the GUI and the underlying data structure.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="turnOrder">Turn order.</param>
	public void RemoveUnit(Unit unit, TurnOrder turnOrder) {
		TurnOrderImage turnOrderImage = _turnOrderImageCollection.Get (unit, turnOrder);
		_turnOrderImageCollection.Remove(turnOrderImage, turnOrder);
		turnOrder.RemoveUnit(unit);
	}

	/// <summary>
	/// Removes the unit and all images.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="turnOrder">Turn order.</param>
	public void RemoveUnitAndAllImages(Unit unit, TurnOrder turnOrder) {
		_turnOrderImageCollection.RemoveAll (unit);
		turnOrder.RemoveUnit (unit);
	}	

	/// <summary>
	/// Reinserts the unit at the specified index.
	/// If index == -1, unit will be added at the end.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Order index.</param>
	public void ReinsertUnit(Unit unit, int orderIndex = -1) {
		RemoveUnit (unit, _turnOrderCollection.Primary);

		// Need to place unit in proper turn order round
		TurnOrder turnOrderRound = _turnOrderCollection.Secondary;
		if (orderIndex > -1) {

			if (orderIndex < (_turnOrderCollection.Primary.GetCount ()))
				turnOrderRound = _turnOrderCollection.Primary;
		}
			
		// Only add to secondary list if doesn't exist
		if (!turnOrderRound.Contains(unit))
			AddUnit(unit, turnOrderRound, orderIndex);

		//TODO: If a deferred ability will take longer than a potential 2nd turn of a unit, remove that unit from a round

		// Sort all the turn orders
		if (orderIndex == -1)
			SortTurnOrder (_turnOrderCollection.Primary);
		SortTurnOrder (_turnOrderCollection.Secondary);
		
		if (orderIndex > -1)
			ReorderIndexes ();
	}

	/// <summary>
	/// Gets the unit that is next up.
	/// </summary>
	/// <returns>The next up.</returns>
	public Unit GetNextUp() {

		// Create the divider at the back of queue if it is at the front
		if (IsDividerInFront ()) {
			_dividerImage.transform.SetSiblingIndex (_turnOrderImageCollection.GetCount(_turnOrderCollection.Secondary));
			_turnOrderCollection.Swap ();
		}

		return _turnOrderCollection.Primary.GetNextUp();
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
		unit.ResetTurnOrderData (_turnOrderCollection.Primary);
		ReinsertUnit (unit, orderIndex);
	}

	/// <summary>
	/// Gets all units.
	/// </summary>
	/// <returns>The all units.</returns>
	public List<Unit> GetAllUnits() {
		List<Unit> a = _turnOrderCollection.Primary.GetAllUnits();
		List<Unit> b = _turnOrderCollection.Secondary.GetAllUnits ();
		return a.Union (b).ToList ();
	}

	/// <summary>
	/// Highlights the unit image with a "selected" color.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void HighlightUnitImage(Unit unit) {
		foreach (var image in _turnOrderImageCollection.GetAll(unit)) {
			image.Highlight (false);
			_highlightedImages.Add (image);
		}
	}

	/// <summary>
	/// Highlights the unit image with a "targeted" color.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void TargetUnitImage(Unit unit) {
		foreach (var image in _turnOrderImageCollection.GetAll(unit)) {
			image.SetTargeted ();
			_targetedImages.Add (image);
		}
	}

	/// <summary>
	/// De-highlight unit image.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void DeHighlightUnitImage() {
		foreach (var highlightedImage in _highlightedImages)
			highlightedImage.DeHighlight ();
		_highlightedImages.Clear ();
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
	/// Gets the turn order collection.
	/// </summary>
	/// <returns>The turn order collection.</returns>
	public TurnOrderCollection GetTurnOrderCollection() {
		return _turnOrderCollection;
	}

	/// <summary>
	/// Reorders the indexes for display purposes.
	/// </summary>
	private void ReorderIndexes() {
		List<Unit> units = _turnOrderCollection.Primary.GetAllUnits();
		units.AddRange(_turnOrderCollection.Secondary.GetAllUnits());
		for (int index = 0; index < units.Count; index++)
			_turnOrderImageCollection.Get (units [index], _turnOrderCollection.Primary).transform.SetSiblingIndex (index);
	}

	/// <summary>
	/// Determines whether the divider is in front of queue.
	/// </summary>
	/// <returns><c>true</c> if this instance is divider in front; otherwise, <c>false</c>.</returns>
	private bool IsDividerInFront() {
		return _dividerImage.transform.GetSiblingIndex () == 0;
	}

	/// <summary>
	/// Adds the divider.
	/// </summary>
	private void AddDivider() {
		_dividerImage.transform.SetParent (_panel.transform, false);
	}

	/// <summary>
	/// Checks if all units are combat ready.
	/// </summary>
	/// <returns><c>true</c>, if all units combat ready was ared, <c>false</c> otherwise.</returns>
	/// <param name="units">Units.</param>
	/// <param name="turnOrder">Turn order.</param>
	private bool AreAllUnitsCombatReady(List<Unit> units, TurnOrder turnOrder) {
		foreach (var unit in units)
			if (unit.IsCombatReady(turnOrder) == false)
				return false;
		return true;
	}

	/// <summary>
	/// Turn order collection.
	/// </summary>
	public class TurnOrderCollection {
		List<TurnOrder> _turnOrders = new List<TurnOrder>();

		/// <summary>
		/// Gets or sets the primary turn order;
		/// </summary>
		/// <value>The primary.</value>
		public TurnOrder Primary {
			get { return _turnOrders [0]; }
			set { _turnOrders.Insert(0, value); }
		}

		/// <summary>
		/// Gets or sets the secondary turn order;
		/// </summary>
		/// <value>The primary.</value>
		public TurnOrder Secondary {
			get { return _turnOrders [1]; }
			set { _turnOrders.Insert(1, value); }
		}

		/// <summary>
		/// Swaps the primary and secondary turn order lists.
		/// </summary>
		public void Swap() {
			var temp = _turnOrders[1];
			_turnOrders.RemoveAt(1);
			_turnOrders.Insert(0, temp);
		}

		/// <summary>
		/// Gets the turn orders.
		/// </summary>
		/// <returns>The turn orders.</returns>
		public List<TurnOrder> GetTurnOrders() {
			return _turnOrders;
		}
	}

	/// <summary>
	/// Turn order image collection.
	/// </summary>
	private class TurnOrderImageCollection {
		private Dictionary<TurnOrder, List<TurnOrderImage>> _imageDictionary = new Dictionary<TurnOrder, List<TurnOrderImage>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TurnOrderController+TurnOrderImageCollection"/> class.
		/// </summary>
		/// <param name="primary">Primary.</param>
		/// <param name="secondary">Secondary.</param>
		public TurnOrderImageCollection(TurnOrder primary, TurnOrder secondary) {
			_imageDictionary[primary] = new List<TurnOrderImage>();
			_imageDictionary[secondary] = new List<TurnOrderImage>();
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		//public int Count { get { return _images.Count; } }

		public int GetCount (TurnOrder turnOrder) {
			return _imageDictionary [turnOrder].Count;
		}

		/// <summary>
		/// Add the specified turnOrderImage and turnOrder.
		/// </summary>
		/// <param name="turnOrderImage">Turn order image.</param>
		/// <param name="turnOrder">Turn order.</param>
		public void Add(TurnOrderImage turnOrderImage, TurnOrder turnOrder) {
			if (!_imageDictionary.ContainsKey (turnOrder))
				_imageDictionary [turnOrder] = new List<TurnOrderImage> ();
			_imageDictionary[turnOrder].Add (turnOrderImage);
		}

		/// <summary>
		/// Get the first turn order image for a unit.
		/// </summary>
		/// <param name="unit">Unit.</param>
		public TurnOrderImage Get(Unit unit, TurnOrder turnOrder) {
			foreach (var image in _imageDictionary[turnOrder])
				if (image.Unit == unit)
					return image;
			return null;
		}

		/// <summary>
		/// Get the specified unit turn order images.
		/// </summary>
		/// <param name="unit">Unit.</param>
		public List<TurnOrderImage> GetAll(Unit unit) {

			// Combine all lists
			List<TurnOrderImage> allImages = new List<TurnOrderImage>();
			foreach (var value in _imageDictionary.Values)
				allImages = allImages.Union (value).ToList ();

			List<TurnOrderImage> imagesToReturn = new List<TurnOrderImage> ();
			foreach (var image in allImages)
				if (image.Unit == unit)
					imagesToReturn.Add (image);
			return imagesToReturn;
		}

		/// <summary>
		/// Remove the specified turnOrderImage.
		/// </summary>
		/// <param name="turnOrderImage">Turn order image.</param>
		public void Remove(TurnOrderImage turnOrderImage, TurnOrder turnOrder) {
			_imageDictionary [turnOrder].Remove (turnOrderImage);	
			turnOrderImage.transform.SetParent(null);
			Destroy (turnOrderImage.gameObject);
		}

		/// <summary>
		/// Removes all images by unit.
		/// </summary>
		/// <param name="unit">Unit.</param>
		public void RemoveAll(Unit unit) {
			Dictionary<TurnOrder, List<TurnOrderImage>> tempImages = new Dictionary<TurnOrder, List<TurnOrderImage>> ();
			foreach (var pair in _imageDictionary) {
				TurnOrder turnOrder = pair.Key;
				var turnOrderImages = pair.Value;
				foreach (var turnOrderImage in turnOrderImages) {
					if (turnOrderImage.Unit == unit) {
						if (!tempImages.ContainsKey (turnOrder))
							tempImages [turnOrder] = new List<TurnOrderImage> ();
						tempImages [turnOrder].Add (turnOrderImage);
					}
				}
			}

			foreach (var pair in tempImages) {
				TurnOrder turnOrder = pair.Key;
				var turnOrderImages = pair.Value;
				foreach (var turnOrderImage in turnOrderImages)
					Remove (turnOrderImage, turnOrder);
			}
		}		
	}
}