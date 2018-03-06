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
		SetAttribute (unit, AttributeEnums.AttributeType.STRENGTH, _strength);
		SetAttribute (unit, AttributeEnums.AttributeType.DEFENSE, _defense);
		SetAttribute (unit, AttributeEnums.AttributeType.DEXTERITY, _dexterity);
		SetAttribute (unit, AttributeEnums.AttributeType.SPEED, _speed);
		SetAttribute (unit, AttributeEnums.AttributeType.MAGIC, _magic);
	}

	/// <summary>
	/// Sets the attribute.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="attributeType">Attribute type.</param>
	/// <param name="text">Text.</param>
	private void SetAttribute(Unit unit, AttributeEnums.AttributeType attributeType, Text text) {
		text.text = unit.GetAttribute (attributeType).CurrentValue.ToString ();
		text.color = Color.black;

		// If attribute has been raised, make green, if lowered, red, else, do nothing
		ModifyAttributeEffect effect = unit.GetModifyAttributeEffect(attributeType);
		if (effect != null) {
			int effectValue = effect.GetValue ();
			if (effectValue < 0)
				text.color = Color.red;
			else if (effectValue > 0)
				text.color = Color.green;
		}
	}
}