using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class TileDiscovererTest {
	
	private const int MAP_SQUARE_SIZE = 10;

	private TileMapData _tileMapData;
	private TileDiscoverer _tileDiscoverer;

	[SetUp]
	public void Setup() {
		_tileMapData = new TileMapData (MAP_SQUARE_SIZE, MAP_SQUARE_SIZE);
		_tileDiscoverer = new TileDiscoverer (_tileMapData);
	}

	[Test]
	public void TestDiscoverTilesInRangeWithoutBoundary() {
		Dictionary<Vector3, Object> tiles = _tileDiscoverer.DiscoverTilesInRange (4, 4, 2);

		Assert.AreEqual (12, tiles.Count);

		// N, E, S, W
		Assert.IsTrue(tiles.ContainsKey(new Vector3(4, 0, 5)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(5, 0, 4)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(4, 0, 3)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(3, 0, 4)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(4, 0, 6)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(6, 0, 4)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(4, 0, 2)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(2, 0, 4)));

		// NE, SE, SW, NW
		Assert.IsTrue(tiles.ContainsKey(new Vector3(5, 0, 5)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(5, 0, 3)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(3, 0, 3)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(3, 0, 5)));
	}

	[Test]
	public void TestDiscoverTilesInRangeWithBoundary() {
		Dictionary<Vector3, Object> tiles = _tileDiscoverer.DiscoverTilesInRange (0, 0, 2);

		Assert.AreEqual (5, tiles.Count);

		Assert.IsTrue(tiles.ContainsKey(new Vector3(0, 0, 1)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(0, 0, 2)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(1, 0, 1)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(1, 0, 0)));
		Assert.IsTrue(tiles.ContainsKey(new Vector3(2, 0, 0)));
	}
}