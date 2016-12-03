using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the turn order of all units participating in battle.
/// </summary>
public class TurnOrder {

	private List<Unit> _combatants;

	/// <summary>
	/// Initializes a new instance of the <see cref="TurnOrder"/> class.
	/// </summary>
	public TurnOrder() {
		_combatants = new List<Unit> ();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TurnOrder"/>class.
	/// </summary>
	/// <param name="combatants">Combatants.</param>
	public TurnOrder(List<Unit> combatants) {
		_combatants = combatants;
	}

	/// <summary>
	/// Adds the combatant.
	/// </summary>
	/// <param name="combatant">Combatant.</param>
	public void AddCombatant(Unit combatant) {
		_combatants.Add(combatant);
	}

	/// <summary>
	/// Removes the combatant.
	/// </summary>
	/// <param name="combatant">Combatant.</param>
	public void RemoveCombatant(Unit combatant) {
		_combatants.Remove (combatant);
	}

	/// <summary>
	/// Gets the unit that is next up.
	/// </summary>
	/// <returns>The next up.</returns>
	public Unit GetNextUp() {
		if (GetTurnOrderLength () > 0)
			return _combatants [0];
		else
			return null;
	}

	/// <summary>
	/// Gets the turn order slot number for a combatant.
	/// </summary>
	/// <returns>The turn order slot.</returns>
	/// <param name="combatant">Combatant.</param>
	public int GetTurnOrderSlotNumber (Unit combatant) {
		return (_combatants.IndexOf (combatant)) + 1;
	}

	/// <summary>
	/// Finishs the turn for the combatant, removing them from the front of the collection and adding them to the back.
	/// </summary>
	/// <param name="combatant">Combatant.</param>
	public void FinishTurn(Unit combatant) {
		RemoveCombatant(combatant);
		AddCombatant(combatant);
	}

	/// <summary>
	/// Gets the turn order length.
	/// </summary>
	/// <returns>The turn order count.</returns>
	public int GetTurnOrderLength() {
		return _combatants.Count;
	}
}