using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class PathfinderTest {

	private const int MAP_SQUARE_SIZE = 10;

	private TileMapData _tileMapData;
	private Node[,] _nodeGrapth;
	private Pathfinder _validStraightPathfinder;
	private Pathfinder _invalidPathfinder;
	private Pathfinder _validIgnoreUnWalkablePathfinder;

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
		tileData [5, 4] = GetWaterTileData ();
		tileData [6, 4] = GetWaterTileData ();

		Graph graph = new Graph (MAP_SQUARE_SIZE, MAP_SQUARE_SIZE);
		graph.Generate4WayGraph ();
		_nodeGrapth = graph.GetGraph ();

		_validStraightPathfinder = new Pathfinder (_tileMapData, _nodeGrapth);
		_validStraightPathfinder.GeneratePath (0, 0, 5, 0);

		_invalidPathfinder = new Pathfinder (_tileMapData, _nodeGrapth);
		_invalidPathfinder.GeneratePath (0, 0, 0, 0);

		_validIgnoreUnWalkablePathfinder = new Pathfinder (_tileMapData, _nodeGrapth);
		_validIgnoreUnWalkablePathfinder.GeneratePath (new Vector3(3, 0, 4), new Vector3(7, 0, 4));
	}

	[Test]
	public void TestGeneratePathValidCount() {
		Assert.AreEqual (6, _validStraightPathfinder.GetGeneratedPath ().Count);
	}

	[Test]
	public void TestGenerateValidStraightPath() {
		foreach (Node node in _validStraightPathfinder.GetGeneratedPath())
			Assert.AreEqual (0, node.z);
	}

	[Test]
	public void TestGeneratePathAvoidUnWalkableTiles() {
		List<Node> generatedPath = _validIgnoreUnWalkablePathfinder.GetGeneratedPath ();

		Assert.AreEqual (7, generatedPath.Count);

		foreach (Node node in generatedPath) {
			TileData tileData = _tileMapData.GetTileDataAt (node.x, node.z);
			Assert.AreEqual (TileData.TerrainTypeEnum.GRASS, tileData.TerrainType);
		}
	}

	[Test]
	public void TestGenerateInvalidPath() {
		Assert.Null(_invalidPathfinder.GetGeneratedPath ());
	}

	[Test]
	public void TestGetGeneratedPathAt() {
		Assert.AreEqual (new Vector3 (0, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (0));
		Assert.AreEqual (new Vector3 (1, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (1));
		Assert.AreEqual (new Vector3 (2, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (2));
		Assert.AreEqual (new Vector3 (3, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (3));
		Assert.AreEqual (new Vector3 (4, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (4));
		Assert.AreEqual (new Vector3 (5, 0, 0), _validStraightPathfinder.GetGeneratedPathAt (5));
	}

	[Test]
	public void TestClear() {
		_validStraightPathfinder.Clear ();

		Assert.AreEqual (0, _validStraightPathfinder.GetGeneratedPath ().Count);
	}

	private TileData GetGrassTileData() {
		return new TileData (TileData.TerrainTypeEnum.GRASS, true, "Grass", 0, 0, 0, 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
	}

	private TileData GetWaterTileData() {
		return new TileData (TileData.TerrainTypeEnum.WATER, false, "Water", 0, 0, 0, 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
	}
}