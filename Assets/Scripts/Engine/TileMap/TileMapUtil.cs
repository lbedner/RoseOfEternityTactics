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
}