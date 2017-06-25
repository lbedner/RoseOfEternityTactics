using UnityEngine;
using System.Collections;

public class Ally : Unit {
	// Blue
	public override Color MovementTileColor { get {return new Color (0.0f, 0.0f, 1.0f, current_highlight_color_transparency);} }
	public override bool IsPlayerControlled { get { return true; } }
	public override Color SelectedColor { get { return Color.green; } }
}