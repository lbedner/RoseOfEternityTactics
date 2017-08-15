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
	public void AddUnit(Unit unit) {
		_units.Add(unit);
	}

	/// <summary>
	/// Inserts the unit at the specified index.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="orderIndex">Order index.</param>
	public void InsertUnit(Unit unit, int orderIndex) {
		_units.Insert (orderIndex, unit);
	}

	/// <summary>
	/// Removes the unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void RemoveUnit(Unit unit) {
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