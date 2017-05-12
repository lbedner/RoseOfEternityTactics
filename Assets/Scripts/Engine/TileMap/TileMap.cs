using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x = 100;
	public int size_z = 50;
	public float tileSize = 1.0f;
	public int tileResolution = 16;

	public Texture2D terrainTiles;

	private TileMapData _tileMapData;
	private Graph _graph;

	private List<Unit> _allies = new List<Unit>();
	private List<Unit> _enemies = new List<Unit> ();

	public void Initialize() {
		print ("TileMap.Initialize()");
		InitializeTileMap ();
		InitUnits ();

		// Generate 4 way pathfinding graph
		_graph = new Graph (size_x, size_z);
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
		BuildMesh();
		BuildTexture ();
	}

	/// <summary>
	/// Builds the mesh.
	/// </summary>
	private void BuildMesh() {
		
		int numTiles = size_x * size_z;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_z = size_z + 1;
		int numVerts = vsize_x * vsize_z;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for(z=0; z < vsize_z; z++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, z*tileSize );
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2( (float)x / size_x, (float)z / size_z );
			}
		}
		//Debug.Log ("Done Verts!");
		
		for(z=0; z < size_z; z++) {
			for(x=0; x < size_x; x++) {
				int squareIndex = z * size_x + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 4] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 5] = z * vsize_x + x + 		   1;
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

		int texWidth = size_x * tileResolution;
		int texHeight = size_z * tileResolution;
		Texture2D texture = new Texture2D (texWidth, texHeight);

		Color[][] tiles = ChopUpTiles ();

		_tileMapData = new TileMapData(size_x, size_z);
		TileData[,] tileData = _tileMapData.GetTileData ();

		tileData = TileMapDataLoader.LoadTileMapData (size_x, size_z, tileData);

		for (int y = 0; y < size_z; y++) {
			for (int x = 0; x < size_x; x++) {
				Color[] p = tiles[(int) tileData[x, y].TerrainType];
				texture.SetPixels (x * tileResolution, y * tileResolution, tileResolution, tileResolution, p);
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
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;

		Color[][] tiles = new Color[numTilesPerRow * numRows][];

		for (int y = 0; y < numRows; y++) {
			for (int x = 0; x < numTilesPerRow; x++) {
				tiles [y * numTilesPerRow + x] = terrainTiles.GetPixels (x * tileResolution, y * tileResolution, tileResolution, tileResolution);
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

	/// <summary>
	/// Inits the units.
	/// </summary>
	private void InitUnits() {
		print ("TileMap.InitUnits()");

		InitUnit ("sinteres", 9, 7);
		InitUnit ("aramus", 10, 7);
		InitUnit ("orelle", 10, 8);
		InitUnit ("jarl", 9, 8);
		InitUnit ("muck_petty", 18, 9);
		InitUnit ("muck_petty", 17, 17);
		InitUnit ("muck_petty", 22, 21);
		InitUnit ("muck_petty", 28, 19);
	}

	/// <summary>
	/// Inits the unit.
	/// </summary>
	/// <param name="resRef">Res reference.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private void InitUnit(string resRef, int x, int z) {
		Unit unit = Unit.InstantiateUnit (resRef);
		if (unit && unit.gameObject.activeSelf) {
			unit.transform.position = TileMapUtil.TileMapToWorldCentered (new Vector3 (x, 0.0f, z), tileSize);
			_tileMapData.GetTileDataAt (x, z).Unit = unit;
			unit.Tile = new Vector3 (x, 0, z);
			GameManager.Instance.GetTurnOrderController ().AddUnit (unit);

			if (unit.UnitData.Type == UnitData.UnitType.PLAYER)
				_allies.Add (unit);
			else if (unit.UnitData.Type == UnitData.UnitType.ENEMY)
				_enemies.Add (unit);
		}
	}
}