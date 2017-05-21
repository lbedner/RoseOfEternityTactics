using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

/// <summary>
/// Converts an exported JSON file from "Tiled" to EE objects.
/// </summary>
public class Tiled2EE {

	public static TileMapData DeserializeObject(string json) {

		JObject jObject = JObject.Parse (json);

		// Get basis tile map data
		int width          = (int) jObject ["width"];
		int height         = (int) jObject ["height"];
		float tileSize     = 5.0f; // TODO: Figure out where to store this
		int tileResolution = (int) jObject ["tileheight"];

		// Get the tile data
		var tileIDs = new List<int> ();
		var unitIDs = new List<int> ();

		// Populate tile/unit id lists
		JArray layers = (JArray)jObject ["layers"];
		foreach (var layer in layers) {
			string layerName = layer ["name"].ToString ();
			if (layerName == "Units")
				foreach (var value in layer["data"])
					unitIDs.Add (value.ToObject<int> ());
			else if (layerName == "Tile Layer 1")
				foreach (var value in layer["data"])
					tileIDs.Add (value.ToObject<int> ());
		}

		// Get offset id for terrain tile set
		int tileOffsetID = (int) jObject["tilesets"] [0] ["firstgid"];
		int unitOffsetID = (int)jObject ["tilesets"] [1] ["firstgid"];

		// Populate tile id > image path dictionary
		var tileImages = new Dictionary<int, string> ();
		JObject tiles = (JObject) jObject["tilesets"][0]["tiles"];
		foreach (KeyValuePair<string, JToken> tile in tiles)
			tileImages [int.Parse(tile.Key)] = tile.Value["image"].ToString();

		// Tile properties
		JObject tileProperties = (JObject) jObject["tilesets"][0]["tileproperties"];
		var tileDataPropertyDictionary = new Dictionary<int, JToken>();
		foreach (KeyValuePair<string, JToken> tileProperty in tileProperties)
			tileDataPropertyDictionary [int.Parse (tileProperty.Key)] = tileProperty.Value;

		// Unit res refs
		JObject unitProperties = (JObject) jObject["tilesets"][1]["tileproperties"];
		var unitPropertyDictionary = new Dictionary<int, string> ();
		foreach (KeyValuePair<string, JToken> unitProperty in unitProperties)
			unitPropertyDictionary [int.Parse (unitProperty.Key)] = unitProperty.Value ["UnitResRef"].ToString ();

		int index = 0;
		TileData[,] tileData = new TileData[width, height];
		for (int z = height - 1; z >= 0; z--) {
			for (int x = 0; x < width; x++) {

				// Get the Tiled tile/unit IDs
				int tileID = tileIDs [index] - tileOffsetID;
				int unitID = unitIDs [index] - unitOffsetID;

				// Get tile data properties
				JToken jToken = tileDataPropertyDictionary [tileID];

				TileData.TerrainTypeEnum terrainType = (TileData.TerrainTypeEnum)Enum.Parse (typeof(TileData.TerrainTypeEnum), jToken ["TerrainType"].ToString ());
				bool isWalkable = Boolean.Parse (jToken ["IsWalkable"].ToString ());
				string name = jToken ["Name"].ToString ();
				int defenseModifier = int.Parse (jToken ["DefenseModifier"].ToString ());
				int dodgeModifier = int.Parse (jToken ["DodgeModifier"].ToString ());
				int accuracyModifier = int.Parse (jToken ["AccuracyModifier"].ToString ());
				int movementModifier = int.Parse (jToken ["MovementModifier"].ToString ());
				Vector3 position = new SerializableVector3 (x, 0.0f, z);
				SerializableVector3 serializablePosition = position;
				string imagePath = Path.GetFileName (tileImages [tileID].ToString ());
				string unitResRef = null;
				if (unitID >= 0)
					unitResRef = unitPropertyDictionary [unitID];

				tileData [x, z] = new TileData (terrainType, isWalkable, name, defenseModifier, dodgeModifier, accuracyModifier, movementModifier, position, serializablePosition, imagePath, unitResRef);

				index++;
			}
		}

		return new TileMapData (width, height, tileSize, tileResolution, "Plains", tileData);
	}
}