using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Holds data for a particular tile, such as the terrain type, if it's walkable, and its name.
/// </summary>
public class TileData {

	/// <summary>
	/// Different terrain types in the game.
	/// </summary>
	public enum TerrainTypeEnum {
		WATER,
		GRASS,
		DESERT,
		MOUNTAINS,
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TileData"/> class.
	/// </summary>
	/// <param name="terrainTypeEnum">Terrain type enum.</param>
	/// <param name="isWalkable">If set to <c>true</c> is walkable.</param>
	/// <param name="name">Name.</param>
	/// <param name="defenseModifier">Defense modifier.</param>
	/// <param name="dodgeModifier">Dodge modifier.</param>
	/// <param name="accuracyModifier">Accuracy modifier.</param>
	/// <param name="movementModifier">Movement modifier.</param>
	/// <param name="position">Position of tile.</param>
	/// <param name="serializabePosition">Serializable position of tile.</param>
	/// <param name="unit">Unit on tile (if any).</param>
	public TileData(
		TerrainTypeEnum terrainTypeEnum,
		bool isWalkable,
		string name,
		int defenseModifier,
		int dodgeModifier,
		int accuracyModifier,
		int movementModifier,
		Vector3 position,
		SerializableVector3 serializablePosition,
		Unit unit = null
	) {
		TerrainType = terrainTypeEnum;
		IsWalkable = isWalkable;
		Name = name;
		DefenseModifier = defenseModifier;
		DodgeModifier = dodgeModifier;
		AccuracyModifier = accuracyModifier;
		MovementModifier = movementModifier;
		SerializablePosition = serializablePosition;
		Position = SerializablePosition;
		Unit = unit;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TileData"/> class.
	/// </summary>
	/// <param name="terrainTypeEnum">Terrain type enum.</param>
	/// <param name="isWalkable">If set to <c>true</c> is walkable.</param>
	/// <param name="name">Name.</param>
	/// <param name="defenseModifier">Defense modifier.</param>
	/// <param name="dodgeModifier">Dodge modifier.</param>
	/// <param name="accuracyModifier">Accuracy modifier.</param>
	/// <param name="movementModifier">Movement modifier.</param>
	/// <param name="serializablePosition">Serializable position.</param>
	/// <param name="unitResRef">Unit res reference.</param>
	[JsonConstructor]
	private TileData (
		TerrainTypeEnum terrainTypeEnum,
		bool isWalkable,
		string name,
		int defenseModifier,
		int dodgeModifier,
		int accuracyModifier,
		int movementModifier,
		SerializableVector3 serializablePosition,
		string unitResRef
	) {
		TerrainType = terrainTypeEnum;
		IsWalkable = isWalkable;
		Name = name;
		DefenseModifier = defenseModifier;
		DodgeModifier = dodgeModifier;
		AccuracyModifier = accuracyModifier;
		MovementModifier = movementModifier;
		SerializablePosition = serializablePosition;
		Position = SerializablePosition;
		UnitResRef = unitResRef;
	}

	private Unit _unit;
		
	/// <summary>
	/// Gets or sets the type of the terrain.
	/// </summary>
	/// <value>The type of the terrain.</value>
	[JsonConverter(typeof(StringEnumConverter))] public TerrainTypeEnum TerrainType { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this tile is walkable.
	/// </summary>
	/// <value><c>true</c> if this instance is walkable; otherwise, <c>false</c>.</value>
	public bool IsWalkable { get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the defense modifier.
	/// </summary>
	/// <value>The defense modifier.</value>
	public int DefenseModifier { get; set; }

	/// <summary>
	/// Gets or sets the dodge modifier.
	/// </summary>
	/// <value>The dodge modifier.</value>
	public int DodgeModifier { get; set; }

	/// <summary>
	/// Gets or sets the accuracy modifier.
	/// </summary>
	/// <value>The accuracy modifier.</value>
	public int AccuracyModifier { get; set; }

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	/// <filterpriority>2</filterpriority>
	public int MovementModifier { get; set; }

	/// <summary>
	/// Gets the position.
	/// </summary>
	/// <value>The position.</value>
	[JsonIgnore] public Vector3 Position { get; private set; }

	/// <summary>
	/// Gets the serializable position.
	/// </summary>
	/// <value>The serializable position.</value>
	public SerializableVector3 SerializablePosition { get; private set; }

	/// <summary>
	/// Gets or sets the unit.
	/// </summary>
	/// <value>The unit.</value>
	[JsonIgnore] public Unit Unit {
		get{
			return _unit;
		}
		set {
			_unit = value;

			string resRef = null;
			if (_unit != null)
				resRef = Unit.ResRef;
			UnitResRef = resRef;
		}
	}

	/// <summary>
	/// Gets the unit resource reference.
	/// </summary>
	/// <value>The unit res reference.</value>
	public string UnitResRef { get; private set; }

	/// <summary>
	/// Swaps units between two tile datas.
	/// This happens when a unit moves from one tile to another.
	/// </summary>
	/// <param name="newTileData">New tile data.</param>
	public void SwapUnits(TileData newTileData) {

		// Update new tile data with unit
		newTileData.Unit = Unit;

		// Clear out old tile data
		Unit = null;
	}

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	/// <filterpriority>2</filterpriority>
	public override string ToString ()
	{
		return string.Format ("[TileData: TerrainType={0}, IsWalkable={1}, Name={2}, DefenseModifier={3}, DodgeModifier={4}, AccuracyModifier={5}, MovementModifier={6}, Position={7}, Unit={8}]", TerrainType, IsWalkable, Name, DefenseModifier, DodgeModifier, AccuracyModifier, MovementModifier, Position, Unit);
	}
}