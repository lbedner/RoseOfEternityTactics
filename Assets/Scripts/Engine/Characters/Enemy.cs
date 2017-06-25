using UnityEngine;
using System.Collections;

public class Enemy : Unit {
	// Red
	public override Color MovementTileColor { get {return new Color (1.0f, 0.0f, 0.0f, current_highlight_color_transparency);}	 }
	public override bool IsPlayerControlled { get { return false; } }
	public override Color SelectedColor { get { return Color.red; } }
}