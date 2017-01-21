using UnityEngine;
using System.Collections;
using RoseOfEternity;

public class AttributeLoader {
	
	/// <summary>
	/// Gets the loaded attributes.
	/// </summary>
	/// <returns>The loaded attributes.</returns>
	public static AttributeCollection GetLoadedAttributes() {
		
		AttributeCollection attributes = new AttributeCollection ();

		// Experience
		AddNewAttribute(attributes, AttributeEnums.AttributeType.EXPERIENCE, "Experience", "XP", "Points until you reach the next level", 0, 0, 100);

		// Level
		AddNewAttribute(attributes, AttributeEnums.AttributeType.LEVEL, "Level", "LVL", "Current level", 1, 1, 100);

		// Hit Points
		AddNewAttribute(attributes, AttributeEnums.AttributeType.HIT_POINTS, "Hit Points", "HP", "Current hit points", 0, 0, 1000);

		// Ability Points
		AddNewAttribute(attributes, AttributeEnums.AttributeType.ABILITY_POINTS, "Ability Points", "AB", "Current ability points", 0, 0, 1000);

		// Movement
		AddNewAttribute(attributes, AttributeEnums.AttributeType.MOVEMENT, "Movement", "MOV", "Number of tiles unit can move to", 0, 0, 1000);

		// Speed
		AddNewAttribute(attributes, AttributeEnums.AttributeType.SPEED, "Speed", "SPD", "Determines how often unit can perform an action", 0, 0, 100);

		return attributes;
	}

	/// <summary>
	/// Adds the new attribute.
	/// </summary>
	/// <param name="attributes">Attributes.</param>
	/// <param name="type">Type.</param>
	/// <param name="name">Name.</param>
	/// <param name="shortName">Short name.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="currentValue">Current value.</param>
	/// <param name="minimumValue">Minimum value.</param>
	/// <param name="maximumValue">Maximum value.</param>
	private static void AddNewAttribute(
		AttributeCollection attributes,
		AttributeEnums.AttributeType type,
		string name,
		string shortName,
		string toolTip,
		float currentValue,
		float minimumValue,
		float maximumValue)
	{
		Attribute attribute = new Attribute (name, shortName, toolTip, currentValue, minimumValue, maximumValue);
		attributes.Add (type, attribute);
	}
}