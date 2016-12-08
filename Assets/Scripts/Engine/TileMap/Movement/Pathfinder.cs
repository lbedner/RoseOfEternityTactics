using UnityEngine;
using System.Collections.Generic;

public class Pathfinder {

	private List<Node> _generatedPath;

	/// <summary>
	/// Generates the path.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="graph">Graph.</param>
	/// <param name="source_x">Source x.</param>
	/// <param name="source_z">Source z.</param>
	/// <param name="target_x">Target x.</param>
	/// <param name="target_z">Target z.</param>
	public List<Node> GeneratePath(Node[,] graph, int source_x, int source_z, int target_x, int target_z) {
	//public static List<Node> GeneratePath(Node[,] graph, int source_x, int source_z, int target_x, int target_z) {

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph [source_x, source_z];
		Node target = graph [target_x, target_z];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
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
				float alt = dist[u] + u.DistanceTo(v);
				//float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
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

		//List<Node> currentPath = new List<Node>();
		_generatedPath = new List<Node>();
		//List<Node> generatedPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			_generatedPath.Add(curr);
			//generatedPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		//generatedPath.Reverse ();
		_generatedPath.Reverse();
		//return generatedPath;
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
		return new Vector3 (_generatedPath[index].x, 0, _generatedPath[index].z);
	}

	/// <summary>
	/// Clear the generated path.
	/// </summary>
	public void Clear() {
		_generatedPath.Clear ();
	}
}