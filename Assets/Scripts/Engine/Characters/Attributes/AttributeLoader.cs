using System.IO;
using UnityEngine;
using System.Collections.Generic;
using RoseOfEternity;
using Newtonsoft.Json;

public class AttributeLoader {
	
	/// <summary>
	/// Gets unit attributes.
	/// </summary>
	/// <returns>The unit attributes.</returns>
	public static AttributeCollection GetUnitAttributes() {
		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		AttributeCollection playerAttributes = new AttributeCollection ();

		// Fetch player attributes from global attribute collection and add to new player collection
		// TODO: Put these hard coded attributes in some sort of unit JSON file
		AttributeEnums.AttributeType[] types = new AttributeEnums.AttributeType[] {
			AttributeEnums.AttributeType.EXPERIENCE,
			AttributeEnums.AttributeType.LEVEL,
			AttributeEnums.AttributeType.HIT_POINTS,
			AttributeEnums.AttributeType.ABILITY_POINTS,
			AttributeEnums.AttributeType.MOVEMENT,
			AttributeEnums.AttributeType.SPEED,
			AttributeEnums.AttributeType.DEXTERITY,
			AttributeEnums.AttributeType.MAGIC,
			AttributeEnums.AttributeType.STRENGTH,
			AttributeEnums.AttributeType.DEFENSE,
		};
		foreach (var type in types)
			playerAttributes.Add (type, globalAttributeCollection.Get (type).DeepCopy ());

		return playerAttributes;
	}

	public static AttributeCollection GetSwordOfGalladoranAttributes() {
		return GetWeaponAttributes (15, 10, 90);
	}

	public static AttributeCollection GetDaishanDaggerAttributes() {
		return GetWeaponAttributes (5, 25, 100);
	}

	public static AttributeCollection GetOrelleWeaponAttributes() {
		return GetWeaponAttributes (12, 20, 70);
	}

	public static AttributeCollection GetJarlWeaponAttributes() {
		return GetWeaponAttributes (4, 5, 100);
	}

	public static AttributeCollection GetMuckWeaponAttributes() {
		return GetWeaponAttributes (2, 8, 95);
	}

	public static AttributeCollection GetDundalanChestAttributes() {
		return GetArmorAttributes (6, 0);
	}

	public static AttributeCollection GetDundalanArmsAttributes() {
		return GetArmorAttributes (3, 0);
	}

	public static AttributeCollection GetDundalanLegsAttributes() {
		return GetArmorAttributes (3, 0);
	}

	public static AttributeCollection GetDundalanShieldAttributes() {
		return GetArmorAttributes (3, 10);
	}

	public static AttributeCollection GetJusticePrevailsChestAttributes() {
		return GetArmorAttributes (5, 5);
	}

	public static AttributeCollection GetJusticePrevailsArmsAttributes() {
		return GetArmorAttributes (2, 2);
	}

	public static AttributeCollection GetJusticePrevailsLegsAttributes() {
		return GetArmorAttributes (2, 1);
	}

	public static AttributeCollection GetJusticePrevailsShieldAttributes() {
		return GetArmorAttributes (2, 15);
	}

	public static AttributeCollection GetLeftRingOfGalladoranAttributes() {
		return GetArmorAttributes (5, 10);
	}

	public static AttributeCollection GetRightRingOfGalladoranAttributes() {
		return GetArmorAttributes (5, 10);
	}

	public static AttributeCollection GetLongBowAttributes() {
		return GetWeaponAttributes (8, 35, 70);
	}

	private static AttributeCollection GetWeaponAttributes(int damage, int criticalPercent, int hitPercent) {

		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		AttributeCollection attributes = new AttributeCollection ();

		AddNewAttribute(attributes, AttributeEnums.AttributeType.DAMAGE, globalAttributeCollection.Get(AttributeEnums.AttributeType.DAMAGE).DeepCopy(), damage);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.HIT_PERCENT, globalAttributeCollection.Get(AttributeEnums.AttributeType.HIT_PERCENT).DeepCopy(), hitPercent);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.CRITICAL_CHANCE, globalAttributeCollection.Get(AttributeEnums.AttributeType.CRITICAL_CHANCE).DeepCopy(), criticalPercent);

		return attributes;
	}

	private static AttributeCollection GetArmorAttributes(int armor, int dodgePercent) {

		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		AttributeCollection attributes = new AttributeCollection ();

		AddNewAttribute(attributes, AttributeEnums.AttributeType.ARMOR, globalAttributeCollection.Get(AttributeEnums.AttributeType.ARMOR).DeepCopy(), armor);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.DODGE_CHANCE, globalAttributeCollection.Get(AttributeEnums.AttributeType.DODGE_CHANCE).DeepCopy(), dodgePercent);

		return attributes;
	}
		

	private static void AddNewAttribute(
		AttributeCollection attributes,
		AttributeEnums.AttributeType type,
		Attribute attribute,
		float currentValue)
	{
		AddNewAttribute (attributes, type, attribute.Name, attribute.ShortName, attribute.ToolTip, currentValue, attribute.MinimumValue, attribute.MaximumValue);
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
		Attribute attribute = new Attribute (type, name, shortName, toolTip, currentValue, minimumValue, maximumValue);
		attributes.Add (type, attribute);
	}
}