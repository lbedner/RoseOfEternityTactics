using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnOrderController : MonoBehaviour {

	public Image imagePrefab;

	private GameObject _panel;

	private Dictionary<Unit, Image> _images = new Dictionary<Unit, Image>();

	private TurnOrder _turnOrder;

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
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void AddUnit(Unit unit) {
		Image unitImage = Instantiate (imagePrefab);
		unitImage.sprite = unit.portrait;
		unitImage.transform.SetParent (_panel.transform, false);

		_images.Add (unit, unitImage);
		_turnOrder.AddCombatant (unit);
	}

	/// <summary>
	/// Removes the unit from the GUI and the underlying data structure.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void RemoveUnit(Unit unit) {
		Image unitImage = _images [unit];
		unitImage.transform.SetParent (null);		
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
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void FinishTurn(Unit unit) {
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
}