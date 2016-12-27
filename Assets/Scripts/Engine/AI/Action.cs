using UnityEngine;
using System.Collections.Generic;

public class Action {
	public Unit Target { get; set; }
	public Vector3 TargetTile { get; set; }
	public Pathfinder Pathfinder { get; set; }

	public override string ToString ()
	{
		return string.Format ("[Action: Target={0}, TargetTile={1}, Pathfinder={2}]", Target, TargetTile, Pathfinder);
	}
}