using UnityEngine;
using System.Collections;

public class TileMapUtil {

	/// <summary>
	/// Converts tile map coordinates to world coordinates.
	/// </summary>
	/// <returns>The map to world.</returns>
	/// <param name="vector">Vector.</param>
	/// <param name="tileSize">Tile size.</param>
	public static Vector3 TileMapToWorld(Vector3 vector, float tileSize) {
		float x = (vector.x * tileSize);
		float z = (vector.z * tileSize);
		return new Vector3 (x, vector.y, z);
	}

	/// <summary>
	/// Converts tile map coordinates to centered world coordinates.
	/// </summary>
	/// <returns>The map to world centered.</returns>
	/// <param name="vector">Vector.</param>
	/// <param name="tileSize">Tile size.</param>
	public static Vector3 TileMapToWorldCentered(Vector3 vector, float tileSize) {
		float x = (vector.x * tileSize) + tileSize / 2.0f;
		float z = (vector.z * tileSize) + tileSize / 2.0f;
		return new Vector3 (x, vector.y, z);
	}

	/// <summary>
	/// Converts world coordinates to tile map coordinates.
	/// </summary>
	/// <returns>The centered to tile map.</returns>
	/// <param name="vector">Vector.</param>
	/// <param name="tileSize">Tile size.</param>
	public static Vector3 WorldCenteredToTileMap(Vector3 vector, float tileSize) {
		int x = Mathf.FloorToInt( vector.x / tileSize);
		int z = Mathf.FloorToInt( vector.z / tileSize);
		return new Vector3 (x, vector.y, z);
	}

	/// <summary>
	/// Converts centered world coordinates to world coordinates.
	/// </summary>
	/// <returns>The centered to uncentered.</returns>
	/// <param name="vector">Vector.</param>
	/// <param name="tileSize">Tile size.</param>
	public static Vector3 WorldCenteredToUncentered(Vector3 vector, float tileSize) {
		float x = vector.x - tileSize / 2.0f;
		float z = vector.z - tileSize / 2.0f;
		return new Vector3 (x, vector.y, z);
	}

	/// <summary>
	/// Gets an invalid tile, which is represented by all vector3 values being infinity.
	/// </summary>
	/// <returns>The invalid vector3.</returns>
	public static Vector3 GetInvalidTile() {
		return new Vector3 (Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	}

	/// <summary>
	/// Determines if the passed in tile is invalid.
	/// </summary>
	/// <returns><c>true</c> if is invalid vector3 the specified vector; otherwise, <c>false</c>.</returns>
	/// <param name="vector">Vector.</param>
	public static bool IsInvalidTile(Vector3 vector) {
		return vector.x == Mathf.Infinity && vector.y == Mathf.Infinity && vector.z == Mathf.Infinity;
	}

	/// <summary>
	/// Determines if passed in coordinates are within the boundaries of the TileMap.
	/// </summary>
	/// <returns><c>true</c> if is inside tile map boundary the specified tileMapData x z; otherwise, <c>false</c>.</returns>
	/// <param name="tileMapData">Tile map data.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public static bool IsInsideTileMapBoundary(TileMapData tileMapData, int x, int z) {
		return x >= 0 && z >= 0 && x < tileMapData.GetWidth () && z < tileMapData.GetHeight ();
	}
}