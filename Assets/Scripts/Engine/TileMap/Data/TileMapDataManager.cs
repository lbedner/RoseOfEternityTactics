using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class TileMapDataManager : MonoBehaviour {

	private const string DATA_FILE = "Data/TileMaps/0000.json";

	private static TileMapDataManager _instance;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static TileMapDataManager Instance { get { return _instance; } }

	/// <summary>
	/// Gets the global inventory.
	/// </summary>
	/// <value>The global inventory.</value>
	public TileMapData GlobalTileMapData { get; private set; }

	/// <summary>
	/// Awake this instance and destroy duplicates.
	/// </summary>
	void Awake() {
		print ("TileMapDataManager.Awake()");
		if (_instance == null) {
			string path = Path.Combine (Application.streamingAssetsPath, DATA_FILE);
			string data = File.ReadAllText (path);

			GlobalTileMapData = JsonConvert.DeserializeObject<TileMapData> (data);
			_instance = this;
		}
		else if (_instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}
}