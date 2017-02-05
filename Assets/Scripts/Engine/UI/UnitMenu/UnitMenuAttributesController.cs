using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitMenuAttributesController : MonoBehaviour {

	[SerializeField] private Text _strength;
	[SerializeField] private Text _defense;
	[SerializeField] private Text _dexterity;
	[SerializeField] private Text _speed;
	[SerializeField] private Text _magic;

	/// <summary>
	/// Sets the attributes.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void SetAttributes(Unit unit) {
		_strength.text = unit.GetAttribute (AttributeEnums.AttributeType.STRENGTH).CurrentValue.ToString ();
		_defense.text = unit.GetAttribute (AttributeEnums.AttributeType.DEFENSE).CurrentValue.ToString ();
		_dexterity.text = unit.GetAttribute (AttributeEnums.AttributeType.DEXTERITY).CurrentValue.ToString ();
		_speed.text = unit.GetAttribute (AttributeEnums.AttributeType.SPEED).CurrentValue.ToString ();
		_magic.text = unit.GetAttribute (AttributeEnums.AttributeType.MAGIC).CurrentValue.ToString ();
	}
}