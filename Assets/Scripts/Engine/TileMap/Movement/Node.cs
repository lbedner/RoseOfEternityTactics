using UnityEngine;
using System.Collections.Generic;

public class Node {
	public List<Node> neighbours;
	public int x;
	public int z;

	/// <summary>
	/// Initializes a new instance of the <see cref="Node"/> class.
	/// </summary>
	public Node() {
		neighbours = new List<Node>();
	}
		
	/// <summary>
	/// Calculates distance from one node to another.
	/// </summary>
	/// <returns>The to.</returns>
	/// <param name="n">N.</param>
	public float DistanceTo(Node n) {
		if(n == null) {
			return 0.0f;
		}

		return Vector2.Distance(
			new Vector2(x, z),
			new Vector2(n.x, n.z)
		);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Node"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Node"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Node: x={0}, y={1}, neighbors={2}]", x, z, neighbours);
	}
}