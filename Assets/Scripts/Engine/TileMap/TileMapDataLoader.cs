using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TileMapDataLoader {

	public static TileData[,] LoadTileMapData(int width, int height, TileData[,] tileData) {

		try {
			List<string> lines = new List<string>();
			string line;
			int y = 0;
			StreamReader reader = new StreamReader("Assets/Data/prototype_map_1.csv");
			using (reader) {
				line = reader.ReadLine();
				if (line != null) {
					do {
						lines.Add(line);
						line = reader.ReadLine();
					}
					while (line != null);
				}
			}

			// Reverse list of lines so they can be loaded from the bottom up
			lines.Reverse();
			foreach (string row in lines) {
				string[] values = row.Split(',');
				for (int x = 0; x < values.Length; x++) {
					tileData[x,y] = ParseTileMapData(values[x]);
				}
				y++;
			}
		}
		catch (System.Exception e) {
			Debug.Log (e.Message);
		}
		return tileData;
	}

	private static TileData ParseTileMapData(string value) {

		// Terrain
		TileData.TerrainTypeEnum terrainType = (TileData.TerrainTypeEnum)int.Parse (value);

		bool isWalkable = false;
		string name = "Unknown";
		int defenseModifier = 0;
		int dodgeModifier = 0;
		int accuracyModifier = 0;
		int movementModifier = 0;

		switch (terrainType) {
		case TileData.TerrainTypeEnum.WATER:
			isWalkable = false;
			name = "Water";
			break;
		case TileData.TerrainTypeEnum.GRASS:
			isWalkable = true;
			name = "Grass";
			break;
		case TileData.TerrainTypeEnum.DESERT:
			isWalkable = true;
			name = "Desert";
			dodgeModifier = -1;
			movementModifier = -2;
			break;
		case TileData.TerrainTypeEnum.MOUNTAINS:
			isWalkable = false;
			name = "Mountains";
			break;
		}

		return new TileData (terrainType, isWalkable, name, defenseModifier, dodgeModifier, accuracyModifier, movementModifier);
	}
}