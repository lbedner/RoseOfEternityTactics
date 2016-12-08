using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the turn order of all units participating in battle.
/// </summary>
public class TurnOrder {

	private List<Unit> _units;

	/// <summary>
	/// Initializes a new instance of the <see cref="TurnOrder"/> class.
	/// </summary>
	public TurnOrder() {
		_units = new List<Unit> ();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TurnOrder"/>class.
	/// </summary>
	/// <param name="units">Units.</param>
	public TurnOrder(List<Unit> units) {
		_units = units;
	}

	/// <summary>
	/// Adds the unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void AddCombatant(Unit unit) {
		_units.Add(unit);
	}

	/// <summary>
	/// Removes the unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void RemoveCombatant(Unit unit) {
		_units.Remove (unit);
	}

	/// <summary>
	/// Gets the unit that is next up.
	/// </summary>
	/// <returns>The next up.</returns>
	public Unit GetNextUp() {
		if (GetTurnOrderLength () > 0)
			return _units [0];
		else
			return null;
	}

	/// <summary>
	/// Gets the turn order slot number for a unit.
	/// </summary>
	/// <returns>The turn order slot.</returns>
	/// <param name="unit">Unit.</param>
	public int GetTurnOrderSlotNumber (Unit unit) {
		return (_units.IndexOf (unit)) + 1;
	}

	/// <summary>
	/// Finishs the turn for the unit, removing them from the front of the collection and adding them to the back.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void FinishTurn(Unit unit) {
		RemoveCombatant(unit);
		AddCombatant(unit);
	}

	/// <summary>
	/// Gets the turn order length.
	/// </summary>
	/// <returns>The turn order count.</returns>
	public int GetTurnOrderLength() {
		return _units.Count;
	}

	/// <summary>
	/// Gets all units.
	/// </summary>
	/// <returns>The all units.</returns>
	public List<Unit> GetAllUnits() {
		return _units;
	}
}