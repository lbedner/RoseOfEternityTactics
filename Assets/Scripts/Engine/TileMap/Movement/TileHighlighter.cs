using UnityEngine;
using System.Collections.Generic;

public class TileHighlighter {
	
	private Dictionary<Vector3, GameObject> _movementTiles = new Dictionary<Vector3, GameObject>();

	private TileMap _tileMap;

	private Transform _movementHighlightCube;

	/// <summary>
	/// Initializes a new instance of the <see cref="TileHighlighter"/>class.
	/// </summary>
	/// <param name="tileMap">Tile map.</param>
	/// <param name="movementHighlightCube">Movement highlight cube.</param>
	public TileHighlighter(TileMap tileMap, Transform movementHighlightCube) {
		_tileMap = tileMap;
		_movementHighlightCube = movementHighlightCube;
	}

	/// <summary>
	/// Highlights the movement tiles for a character.
	/// </summary>
	/// <param name="player">Player.</param>
	public void HighlightMovementTiles(Unit unit, Vector3 unitCurrentTileCoordinate) {

		// Clear out old movement tiles
		RemoveMovementTiles();

		// Get color, depending on the type of unit
		Color movementTileColor = unit.MovementTileColor;

		int movement = unit.movement;
		float x = unitCurrentTileCoordinate.x;
		float z = unitCurrentTileCoordinate.z;

		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= movement; index1++) {
			// Inner loop handles all the other tiles NE, SE, NW, SW
			for (int index2 = 1; index2 <= movement - index1; index2++) {
				InstantiateMovementHightlightCube (movement, index2, x + index1, z + index2, movementTileColor); // North East
				InstantiateMovementHightlightCube (movement, index2, x + index1, z - index2, movementTileColor); // South East
				InstantiateMovementHightlightCube (movement, index2, x - index1, z + index2, movementTileColor); // North West
				InstantiateMovementHightlightCube (movement, index2, x - index1, z - index2, movementTileColor); // South West
			}
			InstantiateMovementHightlightCube (movement, index1, x, z + index1, movementTileColor); // North
			InstantiateMovementHightlightCube (movement, index1, x + index1, z, movementTileColor); // East
			InstantiateMovementHightlightCube (movement, index1, x, z - index1, movementTileColor); // South
			InstantiateMovementHightlightCube (movement, index1, x - index1, z, movementTileColor); // West
		}
	}

	/// <summary>
	/// Removes the movement tiles.
	/// </summary>
	public void RemoveMovementTiles() {
		foreach (var go in _movementTiles.Values)
			GameObject.Destroy (go);
		_movementTiles.Clear ();
	}

	/// <summary>
	/// Determines whether the tile is highlighted or not.
	/// </summary>
	/// <returns><c>true</c> if the tile is highlighted; otherwise, <c>false</c>.</returns>
	/// <param name="tileCoordinates">Tile coordinates.</param>
	public bool IsHighlightedTile(Vector3 tileCoordinates) {
		return _movementTiles.ContainsKey (tileCoordinates);
	}

	/// <summary>
	/// Instantiates the movement hightlight cube.
	/// </summary>
	/// <param name="totalMovement">Total movement.</param>
	/// <param name="currentMovement">Current movement.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="movementTileColor">Movement tile color.</param>
	private void InstantiateMovementHightlightCube(int totalMovement, int currentMovement, float x, float z, Color movementTileColor) {

		// Don't go out of boundary
		if (x < 0 || z < 0 || x >= _tileMap.size_x || z >= _tileMap.size_z )
			return;

		// Don't highlight unwalkable areas (i.e. mountains or occupied tiles)
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt ((int) x, (int) z);
		if (!tileData.IsWalkable || tileData.Unit != null)
			return;

		// Create movement highlight cube and set the material's color
		Vector3 vector = new Vector3 (x, 0, z) * _tileMap.tileSize;
		GameObject movementHightlightCubeClone = GameObject.Instantiate (_movementHighlightCube.gameObject, vector, Quaternion.identity) as GameObject;
		movementHightlightCubeClone.transform.Find ("Cube").gameObject.GetComponent<Renderer> ().material.color = movementTileColor;
		_movementTiles.Add (new Vector3 (x, 0, z), movementHightlightCubeClone);
	}
}