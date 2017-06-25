using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnOrderUnitStatus : MonoBehaviour {
	[SerializeField] private Text _name;
	[SerializeField] private Text _speed;
	[SerializeField] private Text _target;
	[SerializeField] private Text _targetedBy;
	[SerializeField] private Text _status;

	/// <summary>
	/// Activate this instance with unit's attributes.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void Activate(Unit unit) {
		_name.text = unit.GetFullName ();
		_speed.text = string.Format("Speed: {0}", unit.GetSpeedAttribute ().CurrentValue.ToString ());
		_target.text = "Target: N/A";
		_targetedBy.text = "Targeted By: N/A";
		_status.text = "Status: N/A";

		gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate this instance.
	/// </summary>
	public void Deactivate() {
		gameObject.SetActive (false);
	}
}