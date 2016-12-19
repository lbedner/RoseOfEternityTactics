using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class TileMapDataTest {
	
	private const int MAP_SQUARE_SIZE = 10;

	private TileMapData _tileMapData;

	[SetUp]
	public void Setup() {
		_tileMapData = new TileMapData (MAP_SQUARE_SIZE, MAP_SQUARE_SIZE);
		TileData[,] tileData = _tileMapData.GetTileData ();
		for (int x = 0; x < MAP_SQUARE_SIZE; x++) {
			for (int z = 0; z < MAP_SQUARE_SIZE; z++) {
				tileData [x, z] = GetGrassTileData ();
			}
		}

		// Add un-walkable tiles in middle of tilemap
		tileData [4, 4] = GetWaterTileData ();		
	}

	[Test]
	public void TestGetTileAtByCoordinates() {
		TileData tileData = _tileMapData.GetTileDataAt (4, 4);

		RunTileDataAssertions (tileData);
	}

	[Test]
	public void TestGetTileAtByVector() {
		TileData tileData = _tileMapData.GetTileDataAt (new Vector3(4, 0, 4));

		RunTileDataAssertions (tileData);
	}

	[Test]
	public void TestGetTileDataArray() {
		TileData[,] tileDataArray = _tileMapData.GetTileData ();

		Assert.AreEqual (MAP_SQUARE_SIZE, tileDataArray.GetLength (0));
		Assert.AreEqual (MAP_SQUARE_SIZE, tileDataArray.GetLength (1));
	}

	[Test]
	public void TestGetWidth() {
		Assert.AreEqual(MAP_SQUARE_SIZE, _tileMapData.GetWidth());
	}

	[Test]
	public void TestGetHeight() {
		Assert.AreEqual(MAP_SQUARE_SIZE, _tileMapData.GetHeight());
	}

	private void RunTileDataAssertions(TileData tileData) {
		Assert.AreEqual (TileData.TerrainTypeEnum.WATER, tileData.TerrainType);
		Assert.AreEqual (false, tileData.IsWalkable);
		Assert.AreEqual ("Water", tileData.Name);
		Assert.AreEqual (0, tileData.DefenseModifier);
		Assert.AreEqual (0, tileData.DodgeModifier);
		Assert.AreEqual (0, tileData.AccuracyModifier);
		Assert.AreEqual (0, tileData.MovementModifier);
	}		

	private TileData GetGrassTileData() {
		return new TileData (TileData.TerrainTypeEnum.GRASS, true, "Grass", 0, 0, 0, 0);
	}

	private TileData GetWaterTileData() {
		return new TileData (TileData.TerrainTypeEnum.WATER, false, "Water", 0, 0, 0, 0);
	}
}