using UnityEngine;
using System.Collections.Generic;

public class Action {
	public List<Unit> Targets { get; set; }
	public Vector3 TargetTile { get; set; }
	public Pathfinder Pathfinder { get; set; }
	public Ability Ability { get; set; }
	public List<int> DamageToTargets { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Action"/> class.
	/// </summary>
	public Action() {
		ClearTargets ();
	}

	/// <summary>
	/// Clears the targets.
	/// </summary>
	public void ClearTargets() {
		Targets = new List<Unit> ();
		DamageToTargets = new List<int> ();
	}		

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Action"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Action"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Action: Targets={0}, TargetTile={1}, Pathfinder={2}, Ability={3}]", Targets, TargetTile, Pathfinder, Ability);
	}
}