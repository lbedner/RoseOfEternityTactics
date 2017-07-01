using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class AbilityManager : MonoBehaviour {

	private const string ABILITY_DATA_FILE = "Data/abilities.json";

	private static AbilityManager _instance;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static AbilityManager Instance { get { return _instance; } }

	/// <summary>
	/// Gets the global inventory.
	/// </summary>
	/// <value>The global inventory.</value>
	public AbilityCollection GlobalAbilityCollection { get; private set; }

	/// <summary>
	/// Awake this instance and destroy duplicates.
	/// </summary>
	void Awake() {
		print ("AbilityManager.Awake()");
		if (_instance == null) {
			string path = Path.Combine (Application.streamingAssetsPath, ABILITY_DATA_FILE);
			string data = File.ReadAllText (path);

			GlobalAbilityCollection = JsonConvert.DeserializeObject<AbilityCollection>(data);
			_instance = this;
		}
		else if (_instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}
}