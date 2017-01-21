using UnityEngine;
using System.Collections.Generic;

public class ZoneAI : AI {

	/// <summary>
	/// Initializes a new instance of the <see cref="ZoneAI"/> class.
	/// </summary>
	/// <param name="self">Self.</param>
	/// <param name="tileMapData">Tile map data.</param>
	/// <param name="tileDiscoverer">Tile discoverer.</param>
	/// <param name="pathfinder">Pathfinder.</param>
	public ZoneAI(Unit self, TileMapData tileMapData, TileDiscoverer tileDiscoverer, Pathfinder pathfinder) : base(self, tileMapData, tileDiscoverer, pathfinder) {}

	/// <summary>
	/// Gets the target.
	/// </summary>
	/// <returns>The target.</returns>
	protected override Unit GetTarget() {
		return GetNearestEnemy ();
	}

	/// <summary>
	/// Gets the target tile to move to if enmey is within range.
	/// </summary>
	/// <returns>The target tile.</returns>
	/// <param name="targetUnit">Target unit.</param>
	protected override Vector3 GetTargetTile (Unit targetUnit) {

		// Make sure enemy is within range before setting target tile
		Dictionary<Vector3, Object> tiles = _tileDiscoverer.DiscoverTilesInRange (_self.Tile, (int) _self.GetMovementAttribute().CurrentValue + _self.weaponRange);
		if (tiles.ContainsKey (targetUnit.Tile)) {

			// Get target tile
			SetGeneratedPath (_self, targetUnit.Tile);
			return _pathfinder.GetGeneratedPathAt (_pathfinder.GetGeneratedPath ().Count - 1);
		} else
			return _self.Tile;
	}
}