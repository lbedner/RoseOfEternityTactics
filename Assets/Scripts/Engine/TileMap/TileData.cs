/// <summary>
/// Holds data for a particular tile, such as the terrain type, if it's walkable, and its name.
/// </summary>
public class TileData {

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
	/// <param name="unit">Unit on tile (if any).</param>
	public TileData(
		TerrainTypeEnum terrainTypeEnum,
		bool isWalkable,
		string name,
		int defenseModifier,
		int dodgeModifier,
		int accuracyModifier,
		int movementModifier,
		Unit unit = null
	) {
		TerrainType = terrainTypeEnum;
		IsWalkable = isWalkable;
		Name = name;
		DefenseModifier = defenseModifier;
		DodgeModifier = dodgeModifier;
		AccuracyModifier = accuracyModifier;
		MovementModifier = movementModifier;
		Unit = unit;
	}

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
	/// Gets or sets the type of the terrain.
	/// </summary>
	/// <value>The type of the terrain.</value>
	public TerrainTypeEnum TerrainType { get; set; }

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
	/// Gets or sets the unit.
	/// </summary>
	/// <value>The unit.</value>
	public Unit Unit { get; set; }

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
		return string.Format ("[TileData: TerrainType={0}, IsWalkable={1}, Name={2}, DefenseModifier={3}, DodgeModifier={4}, AccuracyModifier={5}, MovementModifier={6}, Unit={7}]", TerrainType, IsWalkable, Name, DefenseModifier, DodgeModifier, AccuracyModifier, MovementModifier, Unit);
	}
}