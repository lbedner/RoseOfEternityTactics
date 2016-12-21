 using UnityEngine;
using System.Collections.Generic;

public class TileDiscoverer {

	private TileMapData _tileMapData;

	private Dictionary<Vector3, Object> _discoveredTiles = new Dictionary<Vector3, Object> ();

	/// <summary>
	/// Initializes a new instance of the <see cref="TileDiscoverer"/> class.
	/// </summary>
	/// <param name="tileMapData">Tile map data.</param>
	public TileDiscoverer(TileMapData tileMapData) {
		_tileMapData = tileMapData;
	}

	/// <summary>
	/// Discovers the tiles in range.
	/// </summary>
	/// <returns>The tiles in range.</returns>
	/// <param name="tile">Tile.</param>
	/// <param name="range">Range.</param>
	public Dictionary<Vector3, Object> DiscoverTilesInRange(Vector3 tile, int range) {
		return DiscoverTilesInRange ((int)tile.x, (int)tile.z, range);
	}

	/// <summary>
	/// Discovers the tiles in range.
	/// </summary>
	/// <returns>The tiles in range.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="range">Range.</param>
	public Dictionary<Vector3, Object> DiscoverTilesInRange(int x, int z, int range) {

		Clear ();

		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= range; index1++) {
			DiscoverTileInRange(x, z + index1);
			DiscoverTileInRange(x + index1, z);
			DiscoverTileInRange(x, z - index1);
			DiscoverTileInRange(x - index1, z);
			
			if (range > 1) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= range - index1; index2++) {
					DiscoverTileInRange(x + index1, z + index2); // North East
					DiscoverTileInRange(x + index1, z - index2); // South East
					DiscoverTileInRange(x - index1, z + index2); // North West
					DiscoverTileInRange(x - index1, z - index2); // South West
				}
			}
		}
		return _discoveredTiles;
	}

	/// <summary>
	/// Discovers the tile in range after some validation.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private void DiscoverTileInRange(int x, int z) {

		// Don't go out of boundary
		if (TileMapUtil.IsInsideTileMapBoundary (_tileMapData, x, z))
			_discoveredTiles.Add (new Vector3 (x, 0, z), null);
	}

	/// <summary>
	/// Clear the discovered tiles.
	/// </summary>
	private void Clear() {
		_discoveredTiles.Clear ();
	}
}