using UnityEngine;
using NUnit.Framework;

public class TileMapUtilTest {

	private const float TILE_SIZE = 2.0f;

	private Vector3 _tileCoordinates;
	private Vector3 _worldCoordinates;
	private Vector3 _worldCoordinatesCentered;
	private Vector3 _invalidTile;

	[SetUp]
	public void Setup() {
		_tileCoordinates = new Vector3 (3, 0, 3);
		_worldCoordinates = new Vector3 (6, 0, 6);
		_worldCoordinatesCentered = new Vector3 (7, 0, 7);
		_invalidTile = new Vector3 (Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	}

	[Test]
	public void TestTileMapToWorld() {
		Vector3 actual = TileMapUtil.TileMapToWorld (_tileCoordinates, TILE_SIZE);
		Assert.AreEqual (_worldCoordinates, actual);
	}

	[Test]
	public void TestTileMapToWorldCentered() {
		Vector3 actual = TileMapUtil.TileMapToWorldCentered (_tileCoordinates, TILE_SIZE);
		Assert.AreEqual (_worldCoordinatesCentered, actual);		
	}

	[Test]
	public void TestWorldCenteredToTileMap() {
		Vector3 actual = TileMapUtil.WorldCenteredToTileMap (_worldCoordinatesCentered, TILE_SIZE);
		Assert.AreEqual (_tileCoordinates, actual);
	}

	[Test]
	public void TestWorldCenteredToUncentered() {
		Vector3 actual = TileMapUtil.WorldCenteredToUncentered (_worldCoordinatesCentered, TILE_SIZE);
		Assert.AreEqual (_worldCoordinates, actual);
	}

	[Test]
	public void TestGetInvalidTile() {
		Vector3 actual = TileMapUtil.GetInvalidTile ();
		Assert.AreEqual (_invalidTile, actual);
	}

	[Test]
	public void TestIsInvalidTile() {
		Assert.IsTrue (TileMapUtil.IsInvalidTile (_invalidTile));
	}
}