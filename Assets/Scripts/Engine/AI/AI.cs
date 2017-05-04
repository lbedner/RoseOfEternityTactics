using UnityEngine;
using System.Collections.Generic;

public abstract class AI {

	protected Unit _self;
	protected TileMapData _tileMapData;
	protected TileDiscoverer _tileDiscoverer;
	protected Pathfinder _pathfinder;

	/// <summary>
	/// Gets the target.
	/// </summary>
	/// <returns>The target.</returns>
	protected abstract Unit GetTarget ();

	/// <summary>
	/// Gets the target tile.
	/// </summary>
	/// <returns>The target tile.</returns>
	/// <param name="targetUnit">Target unit.</param>
	protected abstract Vector3 GetTargetTile (Unit targetUnit);

	/// <summary>
	/// Initializes a new instance of the <see cref="AI"/> class.
	/// </summary>
	/// <param name="self">Self.</param>
	/// <param name="tileMapData">Tile map data.</param>
	/// <param name="tileDiscoverer">Tile discoverer.</param>
	/// <param name="pathfinder">Pathfinder.</param>
	protected AI(Unit self, TileMapData tileMapData, TileDiscoverer tileDiscoverer, Pathfinder pathfinder) {
		_self = self;
		_tileMapData = tileMapData;
		_tileDiscoverer = tileDiscoverer;
		_pathfinder = pathfinder;
	}
		
	/// <summary>
	/// Gets the action.
	/// </summary>
	/// <returns>The action.</returns>
	public Action GetAction() {
		Action action = new Action ();

		Unit target = GetTarget();
		action.TargetTile = GetTargetTile (target);
		action.Pathfinder = GetPathfinder();
		if (IsTargetWithinRange (target))
			action.Target = target;
		return action;
	}

	/// <summary>
	/// s
	/// </summary>
	/// <returns>The pathfinder.</returns>
	protected virtual Pathfinder GetPathfinder () {
		return _pathfinder;
	}

	/// <summary>
	/// Gets the nearest enemy.
	/// </summary>
	/// <returns>The nearest enemy.</returns>
	protected Unit GetNearestEnemy() {
		TurnOrderController turnOrderController = GameManager.Instance.GetTurnOrderController ();
		float currentDistance = Mathf.Infinity;
		Unit nearestEnemy = null;

		foreach (Unit unit in turnOrderController.GetAllUnits()) {
			if (!_self.IsFriendlyUnit (unit)) {
				float distance = Vector3.Distance (unit.Tile, _self.Tile);
				if (distance < currentDistance) {
					currentDistance = distance;
					nearestEnemy = unit;
				}
			}
		}
		//Debug.Log (string.Format("Nearest Enemy - {0}: {1}", nearestEnemy, currentDistance));
		return nearestEnemy;
	}

	/// <summary>
	/// Sets the generated path.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="targetTile">Target tile.</param>
	protected void SetGeneratedPath(Unit unit, Vector3 targetTile) {

		// Generate path to target tile
		_pathfinder.GeneratePath(unit.Tile, targetTile);

		// Get movement tiles for unit so unreachable tiles can be filtered from the generated path
		Dictionary<Vector3, Object> tiles = _tileDiscoverer.DiscoverTilesInRange (unit.Tile, (int) unit.GetMovementAttribute().CurrentValue);

		// Lop off tiles from path that aren't within range or that are occupied
		for (int index = _pathfinder.GetGeneratedPath ().Count - 1; index >= 0; index--) {
			Vector3 tile = _pathfinder.GetGeneratedPathAt (index);
			TileData tileData = _tileMapData.GetTileDataAt ((int) tile.x, (int) tile.z);
			if (!tiles.ContainsKey (_pathfinder.GetGeneratedPathAt (index)) || tileData.Unit)
				_pathfinder.RemoveGeneratedPathAt (index);
			else
				break;						
		}
	}

	/// <summary>
	/// Determines whether the target is within range of the movement and ability.
	/// </summary>
	/// <returns><c>true</c> if the target is within range of the movement and ability; otherwise, <c>false</c>.</returns>
	/// <param name="target">Target.</param>
	private bool IsTargetWithinRange(Unit target) {
		var tilesInRange = _tileDiscoverer.DiscoverTilesInRange (_self.Tile, (int) _self.GetMovementAttribute().CurrentValue + _self.GetWeaponRange());
		return tilesInRange.ContainsKey (target.Tile);
	}
}