using UnityEngine;
using System.Collections.Generic;

public class TileHighlighter {

	// Different types of hightlight types
	private enum HighlightType {
		MOVEMENT,
		ATTACK,
	}

	// Holds the current highlighted tiles
	private Dictionary<Vector3, GameObject> _movementTiles = new Dictionary<Vector3, GameObject>();
	private Dictionary<Vector3, GameObject> _attackTiles   = new Dictionary<Vector3, GameObject>();

	// Used to show "highlighting" of a tile
	private Transform _highlightCube;

	private TileMap _tileMap;

	/// <summary>
	/// Initializes a new instance of the <see cref="TileHighlighter"/>class.
	/// </summary>
	/// <param name="tileMap">Tile map.</param>
	/// <param name="movementHighlightCube">Movement highlight cube.</param>
	public TileHighlighter(TileMap tileMap, Transform highlightCube) {
		_tileMap = tileMap;
		_highlightCube = highlightCube;
	}

	/// <summary>
	/// Removes the movement tiles.
	/// </summary>
	public void RemoveHighlightedTiles() {
		foreach (var go in _movementTiles.Values)
			GameObject.Destroy (go);
		_movementTiles.Clear ();
		foreach (var go in _attackTiles.Values)
			GameObject.Destroy (go);
		_attackTiles.Clear ();
	}

	/// <summary>
	/// Determines whether the tile is highlighted or not.
	/// </summary>
	/// <returns><c>true</c> if the tile is highlighted; otherwise, <c>false</c>.</returns>
	/// <param name="tileCoordinates">Tile coordinates.</param>
	public bool IsHighlightedMovementTile(Vector3 tileCoordinates) {
		return _movementTiles.ContainsKey (tileCoordinates);
	}

	/// <summary>
	/// Highlights the movement and attack tiles for the unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="unitCurrentTileCoordinate">Unit current tile coordinate.</param>
	public void HighlightTiles(Unit unit, Vector3 unitCurrentTileCoordinate) {
		RemoveHighlightedTiles();
		HighlightMovementTiles (unit, unitCurrentTileCoordinate);
		HighlightAttackTiles (unit);
	}

	/// <summary>
	/// Highlights the movement tiles for a unit.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="unitCurrentTileCoordinate">Unit current tile coordinate.</param>
	private void HighlightMovementTiles(Unit unit, Vector3 unitCurrentTileCoordinate) {

		Color tileColor = unit.MovementTileColor;
		HighlightType highlightType = HighlightType.MOVEMENT;
		int range = unit.movement;

		float x = unitCurrentTileCoordinate.x;
		float z = unitCurrentTileCoordinate.z;

		// Highlight current tile unit is standing on
		HighlightTile(unit, x, z, tileColor, highlightType);

		// Highlight the rest of the tiles within range
		HighlightTilesInRange (unit, x, z, range, tileColor, highlightType);
	}

	/// <summary>
	/// Highlights the attack tiles.
	/// </summary>
	/// <param name="unit">Unit.</param>
	private void HighlightAttackTiles(Unit unit) {
	
		Color tileColor = unit.attackTileColor;
		HighlightType highlightType = HighlightType.ATTACK;
		int range = unit.weaponRange;

		// Iterate over all starting tiles where the attack tiles will be highlighted from
		foreach (Vector3 key in _movementTiles.Keys) {

			// We need to attempt to highlight all tiles around the starting tile
			float x = key.x;
			float z = key.z;

			// Highlight the tiles within range
			HighlightTilesInRange (unit, x, z, range, tileColor, highlightType);
		}
	}

	/// <summary>
	/// Highlights the tiles in range.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="range">Range.</param>
	/// <param name="tileColor">Tile color.</param>
	/// <param name="highlightType">Highlight type.</param>
	private void HighlightTilesInRange(Unit unit, float x, float z, int range, Color tileColor, HighlightType highlightType) {

		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= range; index1++) {
			HighlightTile (unit, x, z + index1, tileColor, highlightType);
			HighlightTile (unit, x + index1, z, tileColor, highlightType);
			HighlightTile (unit, x, z - index1, tileColor, highlightType);
			HighlightTile (unit, x - index1, z, tileColor, highlightType);

			if (range > 1) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= range - index1; index2++) {
					HighlightTile (unit, x + index1, z + index2, tileColor, highlightType); // North East
					HighlightTile (unit, x + index1, z - index2, tileColor, highlightType); // South East
					HighlightTile (unit, x - index1, z + index2, tileColor, highlightType); // North West
					HighlightTile (unit, x - index1, z - index2, tileColor, highlightType); // South West
				}
			}
		}
	}

	/// <summary>
	/// Instantiates the movement hightlight cube.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="movementTileColor">Movement tile color.</param>
	/// <param name="isMovement">Determines if this is a tile you can move to.</param>
	private void HighlightTile(Unit unit, float x, float z, Color movementTileColor, HighlightType highlightType) {

		// Don't go out of boundary
		if (!TileMapUtil.IsInsideTileMapBoundary (_tileMap.GetTileMapData (), (int) x, (int) z))
			return;

		// Don't try to overwrite an existing highlighted area
		Vector3 key = new Vector3(x, 0, z);
		if (_attackTiles.ContainsKey (key) || _movementTiles.ContainsKey (key))
			return;

		// Don't highlight orphaned tile if unit can't get to it
		if (IsOrphanedTile (key, highlightType))
			return;

		// Don't highlight unwalkable areas (i.e. mountains) for movements
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt ((int)x, (int)z);
		if (highlightType == HighlightType.MOVEMENT) {
			if (!tileData.IsWalkable || (tileData.Unit && tileData.Unit != unit))
				return;
		}
		// Don't highlight tiles occupied by allies for attacks
		else if (highlightType == HighlightType.ATTACK) {
			if (tileData.Unit && (unit.IsPlayerControlled == tileData.Unit.IsPlayerControlled))
				return;
		}

		// Create highlight cube and set the material's color
		Vector3 vector = new Vector3 (x, 0, z) * _tileMap.tileSize;
		GameObject hightlightCubeClone = GameObject.Instantiate (_highlightCube.gameObject, vector, Quaternion.identity) as GameObject;
		hightlightCubeClone.transform.Find ("Cube").gameObject.GetComponent<Renderer> ().material.color = movementTileColor;

		// Add highlight cube to dictionary based off type
		switch (highlightType) {
		case HighlightType.MOVEMENT:
			_movementTiles.Add (new Vector3 (x, 0, z), hightlightCubeClone);
			break;
		case HighlightType.ATTACK:
			_attackTiles.Add (new Vector3 (x, 0, z), hightlightCubeClone);
			break;
		}
	}

	/// <summary>
	/// Determines whether this tile is orphaned (no other tiles touching it).
	/// </summary>
	/// <returns><c>true</c> if this tile is orphaned; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile.</param>
	/// <param name="highlightType">Highlight type.</param>
	private bool IsOrphanedTile(Vector3 tile, HighlightType highlightType) {

		// Don't perform this check for attack tiles
		if (highlightType == HighlightType.ATTACK)
			return false;

		// If no highlighted tiles exist, this is the first one, so skip the check
		if (_movementTiles.Count <= 0)
			return false;
		
		float x = tile.x;
		float z = tile.z;

		// Check all 8 surrounding tiles for orphanage
		if (!_movementTiles.ContainsKey (new Vector3 (x, 0, z + 1)) &&      // North
			!_movementTiles.ContainsKey (new Vector3 (x + 1, 0, z)) &&      // East
			!_movementTiles.ContainsKey (new Vector3 (x, 0, z - 1)) &&      // South
			!_movementTiles.ContainsKey (new Vector3 (x - 1, 0, z)) &&      // West
			!_movementTiles.ContainsKey (new Vector3 (x + 1, 0, z + 1)) &&  // North East
			!_movementTiles.ContainsKey (new Vector3 (x + 1, 0, z - 1)) &&  // South East
			!_movementTiles.ContainsKey (new Vector3 (x - 1, 0, z + 1)) &&  // North West
			!_movementTiles.ContainsKey (new Vector3 (x - 1, 0, z - 1)))    // South West
		{
			return true;
		}
		return false;
	}
}