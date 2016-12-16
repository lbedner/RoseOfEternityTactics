using UnityEngine;
using NUnit.Framework;
using System.Collections;

public class GraphTest {

	private const int SQUARE_SIZE = 3;

	private Graph _graph;
	private Node[,] _nodeGraph;

	[SetUp]
	public void Setup() {
		_graph = new Graph (SQUARE_SIZE, SQUARE_SIZE);
		_nodeGraph = _graph.GetGraph ();
	}

	[Test]
	public void TestGraphLength() {
		Assert.AreEqual (SQUARE_SIZE * SQUARE_SIZE, _nodeGraph.Length);
	}

	[Test]
	public void TestGraphDimensionalLengths() {
		Assert.AreEqual (SQUARE_SIZE, _nodeGraph.GetLength (0));
		Assert.AreEqual (SQUARE_SIZE, _nodeGraph.GetLength (1));
	}

	[Test]
	public void TestGenerate4WayGraph() {
		_graph.Generate4WayGraph ();

		// Start from middle of graph
		Node node = _nodeGraph[1, 1];
		Assert.AreEqual (4, node.neighbours.Count);
	}

	[Test]
	public void TestGenerate8WayGraph() {
		_graph.Generate8WayGraph ();

		// Start from middle of graph
		Node node = _nodeGraph[1, 1];
		Assert.AreEqual (8, node.neighbours.Count);
	}
}