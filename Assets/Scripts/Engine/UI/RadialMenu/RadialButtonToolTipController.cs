using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialButtonToolTipController : MonoBehaviour {

	[SerializeField] private Text _name;
	[SerializeField] private Text _description;
	[SerializeField] private Text _cost;
	[SerializeField] private Text _turns;
	[SerializeField] private Text _range;
	[SerializeField] private Text _aoeRange;
	[SerializeField] private Text _targetType;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Initialize the specified RadialButtonToolTipController.
	/// </summary>
	/// <param name="radialMenuController">Radial menu controller.</param>
	public void Initialize(RadialMenuController radialMenuController) {
		transform.SetParent (radialMenuController.transform, false);
		transform.localPosition = new Vector3 (32.0f, -240.0f, 0.0f);
	}

	/// <summary>
	/// Activate the tool tip.
	/// </summary>
	/// <param name="ability">Ability.</param>
	public void Activate(Ability ability) {
		_name.text = ability.Name;
		_description.text = ability.Description;
		_cost.text = string.Format ("Cost: {0}", ability.Cost);
		_turns.text = string.Format ("Execution Time: {0}", ability.Turns);
		_range.text = string.Format ("Range: {0}", ability.GetRange ());
		_aoeRange.text = string.Format ("Area of Effect Range: {0}", ability.GetAOERange ());
		_targetType.text = string.Format ("Target Type: {0}", ability.TargetType);

		gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the tool tip.
	/// </summary>
	public void Deactivate() {
		gameObject.SetActive (false);

		_name.text = "";
		_description.text = "";
		_cost.text = "";
		_turns.text = "";
		_range.text = "";
		_aoeRange.text = "";
		_targetType.text = "";
	}		
}