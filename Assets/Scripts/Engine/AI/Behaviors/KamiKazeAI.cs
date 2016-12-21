using UnityEngine;
using System.Collections;

public class KamiKazeAI : AI {

	/// <summary>
	/// Initializes a new instance of the <see cref="KamiKazeAI"/> class.
	/// </summary>
	/// <param name="self">Self.</param>
	/// <param name="tileMapData">Tile map data.</param>
	/// <param name="tileDiscoverer">Tile discoverer.</param>
	/// <param name="pathfinder">Pathfinder.</param>
	public KamiKazeAI(Unit self, TileMapData tileMapData, TileDiscoverer tileDiscoverer, Pathfinder pathfinder) : base(self, tileMapData, tileDiscoverer, pathfinder) {}

	/// <summary>
	/// Gets the target.
	/// </summary>
	/// <returns>The target.</returns>
	protected override Unit GetTarget() {
		return GetNearestEnemy ();
	}

	/// <summary>
	/// Gets the target tile.
	/// </summary>
	/// <returns>The target tile.</returns>
	/// <param name="targetUnit">Target unit.</param>
	protected override Vector3 GetTargetTile (Unit targetUnit) {

		// Get target tile
		SetGeneratedPath(_self, targetUnit.Tile);
		return _pathfinder.GetGeneratedPathAt(_pathfinder.GetGeneratedPath().Count - 1);
	}
}