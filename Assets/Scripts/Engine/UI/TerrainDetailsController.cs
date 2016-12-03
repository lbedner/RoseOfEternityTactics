using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Terrain details controller.
/// </summary>
public class TerrainDetailsController : MonoBehaviour {

	private const string NOT_APPLICABLE = "N/A";

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
		uiDefenseModifier.text = string.Format("Defense Modifier: {0}", isWalkable ? defenseModifier : NOT_APPLICABLE);
		uiDodgeModifier.text = string.Format("Dodge Modifier: {0}", isWalkable ? dodgeModifier : NOT_APPLICABLE);
		uiAccuracyModifier.text = string.Format("Accuracy Modifier: {0}", isWalkable ? accuracyModifier : NOT_APPLICABLE);
		uiMovementModifier.text = string.Format("Movement Modifier: {0}", isWalkable ? movementModifier : NOT_APPLICABLE);

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the terrain details pop up.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive (false);
	}
}