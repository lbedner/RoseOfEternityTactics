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

	public Texture2D terrainTiles;
	public int tileResolution;

	public GameObject terrainDetailsUI;

	public Ally[] allies;
	public Enemy[] enemies;

	public Ally elyse;
	public Ally orelle;
	public Ally aramus;
	public Ally clopon;

	public Enemy goblin1;
	public Enemy goblin2;
	public Enemy goblin3;
	public Enemy goblin4;

	public TurnOrderController turnOrderController;

	private TileMapData _tileMapData;
	private Graph _graph;
	private TurnOrder _turnOrder;
	private CameraController _cameraController;
	
	// Use this for initialization
	void Start () {
	}

	public void Initialize() {
		InitializeUI ();
		BuildMesh();
		InitPlayer ();

		_cameraController = Camera.main.GetComponent ("CameraController") as CameraController;
		_cameraController.Init (size_x * tileSize, size_z * tileSize);

		// Generate 4 way pathfinding graph
		_graph = new Graph (size_x, size_z);
		_graph.Generate4WayGraph ();

		Camera.main.transform.rotation = Quaternion.Euler (90, 0, 0);

		int screen_width = Screen.width;
		int screen_height = Screen.height;

		Camera.main.orthographicSize = screen_width / (((screen_width / screen_height) * 2) * tileResolution);
		//Camera.main.orthographicSize = Screen.height / 2;
		Debug.Log (Camera.main.orthographicSize);
	}

	/// <summary>
	/// Chops up tiles sprite sheet and apply to 2D array of colors that will be used for the main texture.
	/// </summary>
	/// <returns>The up tiles.</returns>
	Color[][] ChopUpTiles() {
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

	void BuildTexture() {

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

		Debug.Log ("Done Texture!");
	}
	
	public void BuildMesh() {
		
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
				//vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, Random.Range(-1f, 1f), z*tileSize );
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, z*tileSize );
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2( (float)x / size_x, (float)z / size_z );
			}
		}
		Debug.Log ("Done Verts!");
		
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
		
		Debug.Log ("Done Triangles!");
		
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
		Debug.Log ("Done Mesh!");

		BuildTexture ();
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
	/// Initializes the UI.
	/// </summary>
	private void InitializeUI() {
		terrainDetailsUI.SetActive (false);
	}

	private void InitPlayer() {
		print ("TileMap.InitPlayer()");
		print (elyse);
		elyse.transform.position = new Vector3 (GetCenteredValue (8), 0.0f, GetCenteredValue (7));
		orelle.transform.position = new Vector3 (GetCenteredValue (10), 0.0f, GetCenteredValue (8));
		aramus.transform.position = new Vector3 (GetCenteredValue (10), 0.0f, GetCenteredValue (7));
		clopon.transform.position = new Vector3 (GetCenteredValue (7), 0.0f, GetCenteredValue (8));
		goblin1.transform.position = new Vector3 (GetCenteredValue (18), 0.0f, GetCenteredValue (9));
		goblin2.transform.position = new Vector3 (GetCenteredValue (17), 0.0f, GetCenteredValue (17));
		goblin3.transform.position = new Vector3 (GetCenteredValue (22), 0.0f, GetCenteredValue (21));
		goblin4.transform.position = new Vector3 (GetCenteredValue (28), 0.0f, GetCenteredValue (19));

		TileData[,] tileData = _tileMapData.GetTileData ();
		tileData [8, 7].Unit = elyse;
		tileData [10, 8].Unit = orelle;
		tileData [10, 7].Unit = aramus;
		tileData [7, 8].Unit = clopon;
		tileData [18, 9].Unit = goblin1;
		tileData [17, 17].Unit = goblin2;
		tileData [22, 21].Unit = goblin3;
		tileData [28, 19].Unit = goblin4;

		turnOrderController.Initialize();
		foreach (Ally ally in allies) {
			print (ally);
			turnOrderController.AddUnit (ally);
		}
		foreach (Enemy enemy in enemies)
			turnOrderController.AddUnit (enemy);
	}

	private float GetCenteredValue(float value) {
		return (value * tileSize) + tileSize / 2.0f;
	}

	public TurnOrderController GetTurnOrderController() {
		return turnOrderController;
	}

	public CameraController GetCameraController() {
		return _cameraController;
	}
		
	private Vector3 TileMapToWorld(Vector3 vector) {
		float x = (vector.x * tileSize);
		float y = (vector.y);
		float z = (vector.z * tileSize);
		print (new Vector3 (x, y, z));
		return new Vector3 (x, y, z);
	}
}