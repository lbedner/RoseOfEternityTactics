using UnityEngine;
using System.Collections.Generic;

public class Action {
	public Unit Target { get; set; }
	public Vector3 TargetTile { get; set; }
	public Pathfinder Pathfinder { get; set; }
}