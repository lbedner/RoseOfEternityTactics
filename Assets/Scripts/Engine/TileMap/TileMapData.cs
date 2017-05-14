using UnityEngine;
using Newtonsoft.Json;
public class TileMapData {

	public int Width { get; private set; }
	public int Height { get; private set; }
	public float TileSize { get; private set; }
	public int TileResolution { get; private set; }
	public TileData[,] TileData { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapData"/>class.
	/// </summary>
	/// <param name="width">Width of tile map.</param>
	/// <param name="height">Height of tile map.</param>
	public TileMapData(int width, int height) {
		TileData = new TileData[width, height];
		Width = width;
		Height = height;
	}
		
	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapData"/> class.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	/// <param name="tileSize">Tile size.</param>
	/// <param name="tileResolution">Tile resolution.</param>
	/// <param name="tileData">Tile data.</param>
	[JsonConstructor]
	private TileMapData(int width, int height, float tileSize, int tileResolution, TileData[,] tileData) {
		Width = width;
		Height = height;
		TileSize = tileSize;
		TileResolution = tileResolution;
		TileData = tileData;
	}

	/// <summary>
	/// This will get the tile map data at the specified coordinates.
	/// </summary>
	/// <param name="x">X coordinate of tile map data.</param>
	/// <param name="y">Y coordinate of tile map data.</param>
	/// <returns>Tile map data at the specified coordinates.</returns>
	public TileData GetTileDataAt(int x, int y) {
		return TileData[x, y];
	}

	/// <summary>
	/// Gets the tile data at vector.
	/// </summary>
	/// <returns>The <see cref="TileData"/>.</returns>
	/// <param name="vector">Vector.</param>
	public TileData GetTileDataAt(Vector3 vector) {
		return TileData [(int) vector.x, (int) vector.z];
	}

	/// <summary>
	/// Gets the tile data.
	/// </summary>
	/// <returns>The tile data.</returns>
	public TileData[,] GetTileData() {
		return TileData;
	}
}