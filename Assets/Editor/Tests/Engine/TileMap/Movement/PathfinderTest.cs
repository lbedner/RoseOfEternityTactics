using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class PathfinderTest {

	private Node[,] _nodeGrapth;
	private Pathfinder _pathfinder;

	[SetUp]
	public void Setup() {
		Graph graph = new Graph (10, 10);
		graph.Generate4WayGraph ();
		_nodeGrapth = graph.GetGraph ();

		_pathfinder = new Pathfinder ();
	}

	[Test]
	public void TestGeneratePathValidCount() {
		_pathfinder.GeneratePath (_nodeGrapth, 0, 0, 5, 0);

		Assert.AreEqual (6, _pathfinder.GetGeneratedPath ().Count);
	}

	[Test]
	public void TestGenerateValidStraightPath() {
		_pathfinder.GeneratePath (_nodeGrapth, 0, 0, 5, 0);

		foreach (Node node in _pathfinder.GetGeneratedPath())
			Assert.AreEqual (0, node.z);
	}

	[Test]
	public void TestGenerateInvalidPath() {
		_pathfinder.GeneratePath (_nodeGrapth, 0, 0, 0, 0);

		Assert.Null(_pathfinder.GetGeneratedPath ());
	}

	[Test]
	public void TestGetGeneratedPathAt() {
		_pathfinder.GeneratePath (_nodeGrapth, 0, 0, 5, 0);

		Assert.AreEqual (new Vector3 (0, 0, 0), _pathfinder.GetGeneratedPathAt (0));
		Assert.AreEqual (new Vector3 (1, 0, 0), _pathfinder.GetGeneratedPathAt (1));
		Assert.AreEqual (new Vector3 (2, 0, 0), _pathfinder.GetGeneratedPathAt (2));
		Assert.AreEqual (new Vector3 (3, 0, 0), _pathfinder.GetGeneratedPathAt (3));
		Assert.AreEqual (new Vector3 (4, 0, 0), _pathfinder.GetGeneratedPathAt (4));
		Assert.AreEqual (new Vector3 (5, 0, 0), _pathfinder.GetGeneratedPathAt (5));
	}

	[Test]
	public void TestClear() {
		_pathfinder.GeneratePath (_nodeGrapth, 0, 0, 5, 0);
		_pathfinder.Clear ();

		Assert.AreEqual (0, _pathfinder.GetGeneratedPath ().Count);
	}
}