using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class TurnOrderUnitStatus : MonoBehaviour {
	[SerializeField] private Text _name;
	[SerializeField] private Text _action;
	[SerializeField] private Text _target;
	[SerializeField] private Text _targetedBy;
	[SerializeField] private Text _status;

	/// <summary>
	/// Activate this instance with unit's attributes.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void Activate(Unit unit) {
		_name.text = unit.GetFullName ();

		// Target(s) if applicable
		if (unit.HasDeferredAbility) {
			_action.text = string.Format ("Action: {0}", unit.Action.Ability.Name);
			_target.text = string.Format ("Target: {0}", GetTargetsAsString (unit.Action.Targets));
		}
		else {
			_action.text = "Action: None";
			_target.text = "Target: None";
		}
		_targetedBy.text = "Targeted By: None";
		_status.text = "Status: None";

		gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate this instance.
	/// </summary>
	public void Deactivate() {
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Gets the targets as string.
	/// </summary>
	/// <returns>The targets as string.</returns>
	/// <param name="targets">Targets.</param>
	private string GetTargetsAsString(List<Unit> targets) {
		StringBuilder sb = new StringBuilder ();
		foreach (var target in targets)
			sb.Append (string.Format("{0} | ", target.GetFullName()));
		return sb.ToString ();	
	}
}