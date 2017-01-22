using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Terrain details controller.
/// </summary>
public class TerrainDetailsController : MonoBehaviour {

	public Text uiTerrain;

	public Text uiDefenseModifier;
	public Text uiDodgeModifier;
	public Text uiAccuracyModifier;
	public Text uiMovementModifier;

	/// <summary>
	/// Activate the terrain details pop up.
	/// </summary>
	/// <param name="tileData">Tile data.</param>
	public void Activate(TileData tileData) {
		Activate(
			tileData.IsWalkable,
			tileData.TerrainType.ToString(),
			tileData.DefenseModifier.ToString(),
			tileData.DodgeModifier.ToString(),
			tileData.AccuracyModifier.ToString(),
			tileData.MovementModifier.ToString()
		);
	}

	/// <summary>
	/// Activate the terrain details pop up.
	/// </summary>
	/// <param name="isWalkable">If set to <c>true</c> is walkable.</param>
	/// <param name="terrain">Terrain.</param>
	/// <param name="defenseModifier">Defense modifier.</param>
	/// <param name="dodgeModifier">Dodge modifier.</param>
	/// <param name="accuracyModifier">Accuracy modifier.</param>
	/// <param name="movementModifier">Movement modifier.</param>
	public void Activate(
		bool isWalkable,
		string terrain,
		string defenseModifier,
		string dodgeModifier,
		string accuracyModifier,
		string movementModifier
	) {
		uiTerrain.text = terrain;

		SetModifier (uiDefenseModifier, defenseModifier);
		SetModifier (uiDodgeModifier, dodgeModifier);
		SetModifier (uiAccuracyModifier, accuracyModifier);
		SetModifier (uiMovementModifier, movementModifier);

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the terrain details pop up.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive (false);
	}

	/// <summary>
	/// Sets the modifier.
	/// </summary>
	/// <param name="modifier">Modifier.</param>
	/// <param name="modifierValue">Modifier value.</param>
	private void SetModifier(Text modifier, string modifierValue) {
		Color color = Color.white;
		int value = int.Parse (modifierValue);

		if (value < 0)
			color = Color.red;
		else if (value > 0)
			color = Color.green;
			
		modifier.text = modifierValue;
		modifier.color = color;
	}
}