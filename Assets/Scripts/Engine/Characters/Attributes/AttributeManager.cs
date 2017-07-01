using UnityEngine;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using EternalEngine;

public class AttributeManager : MonoBehaviour {
	
	private const string ATTRIBUTE_DATA_FILE = "Data/attributes.json";

	private static AttributeManager _instance;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static AttributeManager Instance { get { return _instance; } }

	/// <summary>
	/// Gets the global attribute collection.
	/// </summary>
	/// <value>The global attribute collection.</value>
	public AttributeCollection GlobalAttributeCollection { get; private set; }

	/// <summary>
	/// Awake this instance and destroy duplicates.
	/// </summary>
	void Awake() {
		print ("AttributeManager.Awake()");
		if (_instance == null) {
			string path = Path.Combine (Application.streamingAssetsPath, ATTRIBUTE_DATA_FILE);
			string data = File.ReadAllText (path);

			var attributes = JsonConvert.DeserializeObject<Dictionary<AttributeEnums.AttributeType, Attribute>> (data);
			GlobalAttributeCollection = new AttributeCollection (attributes);
			_instance = this;
		}
		else if (_instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}
}