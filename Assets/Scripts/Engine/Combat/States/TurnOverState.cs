﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnOverState : CombatState {

	private TileMap _tileMap;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("TurnOverState.Enter");
		base.Enter ();
		StartCoroutine(Init ());
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private IEnumerator Init() {

		yield return null;

		// Clear all highlighted turn order images
		controller.TurnOrderController.DeHighlightUnitImage ();

		// If there is no highlighted unit, it means no actual unit performed an actual action the past turn
		if (controller.HighlightedUnit != null) {

			if (controller.HighlightedUnit.IsPlayerControlled)
				controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);

			_tileMap = controller.TileMap;
			TurnOrderController turnOrderController = controller.TurnOrderController;

			// Clear out radial button container list
			controller.RadialButtonContainers.Clear ();

			// Clear existing action targets and reset them
			controller.ClearActionTargets ();

			// Destroy Radial Menu on unit that just finished turn if they have it
			Unit finishedUnit = turnOrderController.GetNextUp ();
			if (finishedUnit.IsPlayerControlled) {
				RadialMenuController radialMenuController = finishedUnit.GetCanvas ().GetComponentInChildren<RadialMenuController> (true);
				if (radialMenuController != null)
					Destroy (radialMenuController.gameObject);
			}
		}

		// If all objectives are complete, end combat
		if (_tileMap.AreAllEnemiesDefeated ()) {
			controller.MusicController.TransitionMusic (true);
			controller.TurnOrderController.DeHighlightUnitImage ();
			controller.ChangeState<DisplayPostCombatStatsState> ();
		}
		else {
			// If there is no highlighted unit, it means no actual unit performed an actual action the past turn
			if (controller.HighlightedUnit != null) {
				Unit unit = controller.HighlightedUnit;
				unit.DeactivateCombatMenu ();
				unit.TileHighlighter.RemoveHighlightedTiles ();

				// Determine where the unit will be placed in the turn order
				int orderIndex = -1;
				if (!unit.HasExecutedDeferredAbility) {
					Action action = unit.Action;
					if (action != null && action.Ability != null && action.Ability.Turns > 0)
						orderIndex = action.Ability.Turns;
				} else
					unit.HasExecutedDeferredAbility = false;
				controller.TurnOrderController.FinishTurn (unit, orderIndex);

				controller.HighlightedUnit.Unselect ();
				controller.HighlightedUnit = null;
			}
			
			controller.ChangeState<InitTurnState> ();
		}
	}
}