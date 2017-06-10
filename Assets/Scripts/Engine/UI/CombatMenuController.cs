﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Combat menu controller.
/// </summary>
public class CombatMenuController : MonoBehaviour {

	public Text uiMove;
	public Text uiAttack;
	public Text uiAbility;
	public Text uiItem;
	public Text uiEndTurn;

	public Button move;
	public Button attack;
	public Button ability;
	public Button item;
	public Button endTurn;

	/// <summary>
	/// Activate the combat menu.
	/// </summary>
	/// <param name="move">Move.</param>
	/// <param name="attack">Attack.</param>
	/// <param name="ability">Ability.</param>
	/// <param name="item">Item.</param>
	/// <param name="endTurn">End turn.</param>
	public void Activate(
		string move,
		string attack,
		string ability,
		string item,
		string endTurn
	) {
		uiMove.text = move;
		uiAttack.text = attack;
		uiAbility.text = ability;
		uiItem.text = item;
		uiEndTurn.text = endTurn;

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the combat menu.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive(false);
	}
}