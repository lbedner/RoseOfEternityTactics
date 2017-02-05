using UnityEngine;
using System.Collections;
using RoseOfEternity;

public class AttributeLoader {

	private static Attribute _strengthAttribute = new Attribute ("Strength", "STR", "Strength Description.", 10, 1, 100);
	private static Attribute _criticalChanceAttribute = new Attribute ("Critical Chance", "Crit %", "Chance of critical attack.", 10, 0, 100);

	private static Attribute _defenseAttribute = new Attribute ("Defense", "DEF", "Determines how much damage is inflicted upon the unit.", 10, 0, 100);
	private static Attribute _dodgeChanceAttribute = new Attribute ("Dodge Chance", "Dodge %", "Chance of dodging attack.", 10, 0, 100);

	private static Attribute _damageAttribute = new Attribute ("Damage", "DMG", "Damage Description.", 1, 1, 100);
	private static Attribute _hitAttribute = new Attribute ("Hit %", "HIT", "Hit % Description.", 1, 1, 100);
	private static Attribute _armorAttribute = new Attribute ("Armor", "ARM", "Armor Description.", 1, 0, 100);
	
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

		// Dexterity
		AddNewAttribute(attributes, AttributeEnums.AttributeType.DEXTERITY, "Dexterity", "DEX", "Dexterity Description", 1, 1, 100);

		// Magic
		AddNewAttribute(attributes, AttributeEnums.AttributeType.MAGIC, "Magic", "MAG", "Magic Description", 1, 1, 100);

		// Strength
		AddNewAttribute(attributes, AttributeEnums.AttributeType.STRENGTH, _strengthAttribute, 10);

		// Defense
		AddNewAttribute(attributes, AttributeEnums.AttributeType.DEFENSE, _defenseAttribute, 10);

		return attributes;
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

		AttributeCollection attributes = new AttributeCollection ();

		AddNewAttribute(attributes, AttributeEnums.AttributeType.DAMAGE, _damageAttribute, damage);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.HIT_PERCENT, _hitAttribute, hitPercent);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.CRITICAL_CHANCE, _criticalChanceAttribute, criticalPercent);

		return attributes;
	}

	private static AttributeCollection GetArmorAttributes(int armor, int dodgePercent) {

		AttributeCollection attributes = new AttributeCollection ();

		AddNewAttribute(attributes, AttributeEnums.AttributeType.ARMOR, _armorAttribute, armor);
		AddNewAttribute(attributes, AttributeEnums.AttributeType.DODGE_CHANCE, _dodgeChanceAttribute, dodgePercent);

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
		Attribute attribute = new Attribute (name, shortName, toolTip, currentValue, minimumValue, maximumValue);
		attributes.Add (type, attribute);
	}
}