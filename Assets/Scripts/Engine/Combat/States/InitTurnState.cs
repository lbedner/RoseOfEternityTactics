using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitTurnState : CombatState {

	private TileMap _tileMap;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("InitTurnState.Enter");
		base.Enter ();
		StartCoroutine(Init ());
	}

	private IEnumerator Init() {
		_tileMap = controller.TileMap;

		controller.ShowCursorAndTileSelector (false);
		controller.SelectionIndicator.ClearIndicators ();
		controller.MissionObjectivesPanel.SetActive (false);

		Unit unit = controller.TurnOrderController.GetNextUp ();
		StartCoroutine (controller.CameraController.MoveToPosition (unit.transform.position));
		controller.HighlightCharacter (unit);

		TurnOrderController turnOrderController = controller.TurnOrderController;
		MusicController musicController = controller.MusicController;

		if (IsEnemyNearby (turnOrderController.GetAllUnits ()))
			musicController.TransitionMusic (false);
		else
			musicController.TransitionMusic (true);

		// Incremental Effects
		List<Effect> effects = unit.GetEffects();
		if (effects != null && effects.Count > 0) {

			// Lower music so you can hear ability SFX
			yield return StartCoroutine (controller.MusicController.LowerCombatMusic ());

			int index = 0;
			do {
				Effect effect = effects [index];

				// Only run this logic with over time effects
				if (effect.GetEffectType() == EffectType.OVER_TIME) {
					bool showedEffectPopupText = false;
					effect.ApplyIncrementalEffect (unit);

					List<GameObject> vfxGameObjects = ApplyVFXToTargets(effect.GetVFXPath(), new List<Unit>() { unit });
					if (effect.GetDisplayString() != "") {
						showedEffectPopupText = true;
						PopupTextController.CreatePopupText (effect.GetIncrementalDisplayString(), unit.transform.position, effect.GetColor());
					}

					// If unit is killed after application of effect, handle death
					unit.UpdateHealthbar();

					// If pop up text was shown, wait x seconds so it doesn't stack with other potential ones
					if (showedEffectPopupText)
						yield return new WaitForSeconds (2.0f);
					
					if (unit.IsDead())
						HandleDeath(unit);

					DestroyVFX(vfxGameObjects);
				}

				index++;
			} while (index < effects.Count && unit != null);

			// Bring music back up to normal volume
			yield return StartCoroutine (controller.MusicController.RaiseCombatMusic ());
		}

		// Decrement Effect Turns
		if (unit != null)
			unit.DecrementEffectTurns();

		yield return null;

		// If unit is not available at this point, switch state to next unit in the turn order
		if (unit == null)
			controller.ChangeState<TurnOverState> ();
		else {
			if (unit.IsPlayerControlled)
				controller.ChangeState<PlayerTurnState> ();
			else
				controller.ChangeState<CPUTurnState> ();
		}
	}

	/// <summary>
	/// Determines whether there are nearby enemies.
	/// </summary>
	/// <returns><c>true</c> if there are nearby enemies; otherwise, <c>false</c>.</returns>
	/// <param name="units">Units.</param>
	private bool IsEnemyNearby(List<Unit> units) {
		foreach (Unit unit in units) {
			int range = (int) unit.GetMovementAttribute().CurrentValue + unit.GetWeaponRange();
			Vector3 tileMapPosition = TileMapUtil.WorldCenteredToTileMap (unit.transform.position, _tileMap.TileSize);
			int x = (int) tileMapPosition.x;
			int z = (int) tileMapPosition.z;

			// Outer loop handles the straight lines going N, E, S, W
			for (int index1 = 1; index1 <= range; index1++) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= range - index1; index2++) {
					if (IsEnemyNearby (unit, x + index1, z + index2)) // North East
						return true;
					if (IsEnemyNearby (unit, x + index1, z - index2)) // South East
						return true;
					if (IsEnemyNearby (unit, x - index1, z + index2)) // North West
						return true; 
					if (IsEnemyNearby (unit, x - index1, z - index2)) // South West
						return true; 
				}
				if (IsEnemyNearby (unit, x, z + index1)) // North
					return true; 
				if (IsEnemyNearby (unit, x + index1, z)) // East
					return true; 
				if (IsEnemyNearby (unit, x, z - index1)) // South
					return true; 
				if (IsEnemyNearby (unit, x - index1, z)) // West
					return true; 
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether an enemy is nearby the unit.
	/// </summary>
	/// <returns><c>true</c> if this instance is enemy nearby the specified unit x z; otherwise, <c>false</c>.</returns>
	/// <param name="unit">Unit.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private bool IsEnemyNearby(Unit unit, int x, int z) {

		// Don't go out of boundary
		if (!TileMapUtil.IsInsideTileMapBoundary (_tileMap.GetTileMapData (), x, z))
			return false;

		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (x, z);
		Unit targetUnit = tileData.Unit;

		if (targetUnit != null) {
			if (targetUnit.IsPlayerControlled != unit.IsPlayerControlled)
				return true;
		}
		return false;
	}
}