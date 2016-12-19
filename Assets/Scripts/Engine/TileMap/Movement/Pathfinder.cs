using UnityEngine;
using System.Collections.Generic;

public class Pathfinder {

	private List<Node> _generatedPath;
	private TileMapData _tileMapData;
	private Node[,] _graph;

	/// <summary>
	/// Initializes a new instance of the <see cref="Pathfinder"/> class.
	/// </summary>
	/// <param name="tileMapData">Tile map data.</param>
	/// <param name="graph">Graph.</param>
	public Pathfinder(TileMapData tileMapData, Node[,] graph) {
		_tileMapData = tileMapData;
		_graph = graph;
	}

	/// <summary>
	/// Generates the path.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="source_x">Source x.</param>
	/// <param name="source_z">Source z.</param>
	/// <param name="target_x">Target x.</param>
	/// <param name="target_z">Target z.</param>
	public List<Node> GeneratePath(int source_x, int source_z, int target_x, int target_z) {

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = _graph [source_x, source_z];
		Node target = _graph [target_x, target_z];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in _graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				float alt = dist[u] + ((CostToEnterTile(u.x, u.z, v.x, v.z) + 1) * u.DistanceTo(v));
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return new List<Node>();
		}

		_generatedPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			_generatedPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!
		_generatedPath.Reverse();
		return _generatedPath;
	}

	/// <summary>
	/// Gets the generated path.
	/// </summary>
	/// <returns>The generated path.</returns>
	public List<Node> GetGeneratedPath() {
		return _generatedPath;
	}

	/// <summary>
	/// Gets the generated path at the specified index;
	/// </summary>
	/// <returns>The generated path at the specified index.</returns>
	/// <param name="index">Index.</param>
	public Vector3 GetGeneratedPathAt(int index) {
		if (_generatedPath == null || _generatedPath.Count <= 0)
			return TileMapUtil.GetInvalidTile ();
		return new Vector3 (_generatedPath[index].x, 0, _generatedPath[index].z);
	}

	/// <summary>
	/// Clear the generated path.
	/// </summary>
	public void Clear() {
		if (_generatedPath != null)
			_generatedPath.Clear ();
	}

	/// <summary>
	/// Costs to enter tile.
	/// </summary>
	/// <returns>The to enter tile.</returns>
	/// <param name="sourceX">Source x.</param>
	/// <param name="sourceZ">Source z.</param>
	/// <param name="targetX">Target x.</param>
	/// <param name="targetZ">Target z.</param>
	private float CostToEnterTile(int sourceX, int sourceZ, int targetX, int targetZ) {

		TileData tileData = _tileMapData.GetTileDataAt (targetX, targetZ);

		if (!tileData.IsWalkable)
			return Mathf.Infinity;

		float cost = Mathf.Abs (tileData.MovementModifier);

		if( sourceX!=targetX && sourceZ!=targetZ) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}
		return cost;
	}
}