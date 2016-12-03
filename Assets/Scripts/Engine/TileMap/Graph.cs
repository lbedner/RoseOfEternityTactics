using System.Collections;

/// <summary>
/// Graph.
/// </summary>
public class Graph {
	private Node[,] _graph;
	private int _size_x;
	private int _size_z;

	/// <summary>
	/// Initializes a new instance of the <see cref="Graph"/>class.
	/// </summary>
	/// <param name="size_x">Size x.</param>
	/// <param name="size_z">Size z.</param>
	public Graph(int size_x, int size_z) {
		_size_x = size_x;
		_size_z = size_z;
		InitializeGraph ();
	}

	/// <summary>
	/// Gets the graph.
	/// </summary>
	/// <returns>The graph.</returns>
	public Node[,] GetGraph() { return _graph; }

	/// <summary>
	/// Generates a 4 way graph.
	/// </summary>
	public void Generate4WayGraph() {
		for (int x = 0; x < _size_x; x++) {
			for (int z = 0; z < _size_z; z++) {
				if (x > 0)
					_graph[x, z].neighbours.Add(_graph[x - 1, z] );
				if (x < _size_x - 1)
					_graph[x , z].neighbours.Add(_graph[x + 1, z] );
				if (z > 0)
					_graph[x, z].neighbours.Add(_graph[x, z - 1] );
				if (z < _size_z - 1)
					_graph[x, z].neighbours.Add(_graph[x, z + 1] );
			}
		}
	}
		
	/// <summary>
	/// Generates an 8 way graph.
	/// </summary>
	public void Generate8WayGraph() {
		for (int x = 0; x < _size_x; x++) {
			for (int z = 0; z < _size_z; z++) {
				
				// Try West
				if (x > 0) {
					_graph [x, z].neighbours.Add (_graph [x - 1, z]);
					if (z > 0)
						_graph [x, z].neighbours.Add (_graph [x - 1, z - 1]);
					if (z < _size_z - 1)
						_graph [x, z].neighbours.Add (_graph [x - 1, z + 1]);
				}

				// Try East
				if (x < _size_x - 1) {
					_graph [x, z].neighbours.Add (_graph [x + 1, z]);
					if (z > 0)
						_graph [x, z].neighbours.Add (_graph [x + 1, z - 1]);
					if (z < _size_z - 1)
						_graph [x, z].neighbours.Add (_graph [x + 1, z + 1]);
				}

				// Try North/South
				if (z > 0)
					_graph [x, z].neighbours.Add (_graph [x, z - 1]);
				if (z < _size_z - 1)
					_graph [x, z].neighbours.Add (_graph [x, z + 1]);
			}
		}
	}

	/// <summary>
	/// Initializes the graph.
	/// </summary>
	private void InitializeGraph() {
		_graph = new Node[_size_x, _size_z];

		for (int x = 0; x < _size_x; x++) {
			for (int z = 0; z < _size_z; z++) {
				_graph [x, z] = new Node ();
				_graph [x, z].x = x;
				_graph [x, z].z = z;
			}
		}
	}
}