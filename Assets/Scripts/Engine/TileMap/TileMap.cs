using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {

	public int Width { get; private set; }
	public int Height { get; private set; }
	public float TileSize { get; private set; }
	public int TileResolution { get; private set; }

	public Texture2D terrainTiles;

	private TileMapData _tileMapData;
	private Graph _graph;

	private List<Unit> _allies = new List<Unit>();
	private List<Unit> _enemies = new List<Unit> ();

	public void Initialize() {
		print ("TileMap.Initialize()");
		InitializeTileMap ();

		// Generate 4 way pathfinding graph
		_graph = new Graph (Width, Height);
		_graph.Generate4WayGraph ();
	}

	/// <summary>
	/// Gets the allies.
	/// </summary>
	/// <returns>The allies.</returns>
	public List<Unit> GetAllies() {
		return _allies;
	}

	/// <summary>
	/// Gets the enemies.
	/// </summary>
	/// <returns>The enemies.</returns>
	public List<Unit> GetEnemies() {
		return _enemies;
	}

	/// <summary>
	/// Ares all enemies defeated.
	/// </summary>
	/// <returns><c>true</c>, if all enemies defeated was ared, <c>false</c> otherwise.</returns>
	public bool AreAllEnemiesDefeated() {
		foreach (Unit unit in _enemies) {
			if (unit != null)
				return false;
		}
		return true;
	}

	/// <summary>
	/// Initializes the tile map.
	/// </summary>
	public void InitializeTileMap() {
		_tileMapData = TileMapDataManager.Instance.GlobalTileMapData;

		Width = _tileMapData.Width;
		Height = _tileMapData.Height;
		TileSize = _tileMapData.TileSize;
		TileResolution = _tileMapData.TileResolution;

		BuildMesh();
		BuildTexture ();
	}

	/// <summary>
	/// Builds the mesh.
	/// </summary>
	private void BuildMesh() {
		
		int numTiles = Width * Height;
		int numTris = numTiles * 2;
		
		int vWidth = Width + 1;
		int vHeight = Height + 1;
		int numVerts = vWidth * vHeight;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for(z=0; z < vHeight; z++) {
			for(x=0; x < vWidth; x++) {
				vertices[ z * vWidth + x ] = new Vector3( x*TileSize, 0, z*TileSize );
				normals[ z * vWidth + x ] = Vector3.up;
				uv[ z * vWidth + x ] = new Vector2( (float)x / Width, (float)z / Height );
			}
		}
		//Debug.Log ("Done Verts!");
		
		for(z=0; z < Height; z++) {
			for(x=0; x < Width; x++) {
				int squareIndex = z * Width + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vWidth + x + 		   0;
				triangles[triOffset + 1] = z * vWidth + x + vWidth + 0;
				triangles[triOffset + 2] = z * vWidth + x + vWidth + 1;
				
				triangles[triOffset + 3] = z * vWidth + x + 		   0;
				triangles[triOffset + 4] = z * vWidth + x + vWidth + 1;
				triangles[triOffset + 5] = z * vWidth + x + 		   1;
			}
		}
		
		//Debug.Log ("Done Triangles!");
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		//Debug.Log ("Done Mesh!");
	}

	/// <summary>
	/// Builds the texture.
	/// </summary>
	private void BuildTexture() {

		int texWidth = Width * TileResolution;
		int texHeight = Height * TileResolution;
		Texture2D texture = new Texture2D (texWidth, texHeight);

		Color[][] tiles = ChopUpTiles ();

		TileData[,] tileData = _tileMapData.GetTileData ();

		for (int y = 0; y < Height; y++) {
			for (int x = 0; x < Width; x++) {
				TileData currentTileData = tileData [x, y];

				// Set pixels
				Color[] p = tiles[(int) currentTileData.TerrainType];
				texture.SetPixels (x * TileResolution, y * TileResolution, TileResolution, TileResolution, p);

				// If a unit is on the tile, instantiate them and move them to that tile
				if (currentTileData.UnitResRef != null) {
					Unit unit = Unit.InstantiateUnit (currentTileData.UnitResRef);
					currentTileData.Unit = unit;
					unit.Tile = currentTileData.Position;

					unit.transform.position = TileMapUtil.TileMapToWorldCentered (currentTileData.Position, TileSize);
					GameManager.Instance.GetTurnOrderController ().AddUnit (unit);

					if (unit.UnitData.Type == UnitData.UnitType.PLAYER)
						_allies.Add (unit);
					else if (unit.UnitData.Type == UnitData.UnitType.ENEMY)
						_enemies.Add (unit);
				}
			}
		}

		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();

		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials [0].mainTexture = texture;

		//Debug.Log ("Done Texture!");
	}	
		
	/// <summary>
	/// Chops up tiles sprite sheet and apply to 2D array of colors that will be used for the main texture.
	/// </summary>
	/// <returns>The up tiles.</returns>
	private Color[][] ChopUpTiles() {
		int numTilesPerRow = terrainTiles.width / TileResolution;
		int numRows = terrainTiles.height / TileResolution;

		Color[][] tiles = new Color[numTilesPerRow * numRows][];

		for (int y = 0; y < numRows; y++) {
			for (int x = 0; x < numTilesPerRow; x++) {
				tiles [y * numTilesPerRow + x] = terrainTiles.GetPixels (x * TileResolution, y * TileResolution, TileResolution, TileResolution);
			}
		}

		return tiles;
	}

	/// <summary>
	/// Gets the tile map data.
	/// </summary>
	/// <returns>The tile map data.</returns>
	public TileMapData GetTileMapData() {
		return _tileMapData;
	}

	/// <summary>
	/// Gets the graph.
	/// </summary>
	/// <returns>The graph.</returns>
	public Graph GetGraph() {
		return _graph;
	}
}