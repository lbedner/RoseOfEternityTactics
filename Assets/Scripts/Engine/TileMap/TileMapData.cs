using UnityEngine;
public class TileMapData {
	
	private TileData[,] _tileData;

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapData"/>class.
	/// </summary>
	/// <param name="width">Width of tile map.</param>
	/// <param name="height">Height of tile map.</param>
	public TileMapData(int width, int height) {	
		_tileData = new TileData[width, height];
	}

	/// <summary>
	/// This will get the tile map data at the specified coordinates.
	/// </summary>
	/// <param name="x">X coordinate of tile map data.</param>
	/// <param name="y">Y coordinate of tile map data.</param>
	/// <returns>Tile map data at the specified coordinates.</returns>
	public TileData GetTileDataAt(int x, int y) {
		return _tileData[x, y];
	}

	/// <summary>
	/// Gets the tile data at vector.
	/// </summary>
	/// <returns>The <see cref="TileData"/>.</returns>
	/// <param name="vector">Vector.</param>
	public TileData GetTileDataAt(Vector3 vector) {
		return _tileData [(int) vector.x, (int) vector.z];
	}

	/// <summary>
	/// Gets the tile data.
	/// </summary>
	/// <returns>The tile data.</returns>
	public TileData[,] GetTileData() {
		return _tileData;
	}
}