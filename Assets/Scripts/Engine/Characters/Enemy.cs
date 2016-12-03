using UnityEngine;
using System.Collections;

public class Enemy : Unit {
	// Red
	public override Color MovementTileColor { get {return new Color (1.0f, 0.0f, 0.0f, HIGHTLIGHT_COLOR_TRANSPARENCY);}	 }
	public override bool IsPlayerControlled { get { return false; } }
}