using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RoseOfEternity;

public class ItemManager : MonoBehaviour {

	private const string ITEM_DATA_FILE = "Data/items.json";

	private static ItemManager _instance;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static ItemManager Instance { get { return _instance; } }

	/// <summary>
	/// Gets the global inventory.
	/// </summary>
	/// <value>The global inventory.</value>
	public Inventory GlobalInventory { get; private set; }

	/// <summary>
	/// Awake this instance and destroy duplicates.
	/// </summary>
	void Awake() {
		print ("ItemManager.Awake()");
		if (_instance == null) {
			string path = Path.Combine (Application.streamingAssetsPath, ITEM_DATA_FILE);
			string data = File.ReadAllText (path);

			GlobalInventory = JsonConvert.DeserializeObject<Inventory> (data);
			_instance = this;
		}
		else if (_instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}
}