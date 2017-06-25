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

		yield return null;
		if (unit.IsPlayerControlled)
			controller.ChangeState<PlayerTurnState> ();
		else
			controller.ChangeState<CPUTurnState> ();
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