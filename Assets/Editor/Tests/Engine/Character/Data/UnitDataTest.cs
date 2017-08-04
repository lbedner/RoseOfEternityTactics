using UnityEngine;

using NUnit.Framework;

using EternalEngine;

[TestFixture]
public class UnitDataTest {

	private AttributeCollection _ac = new AttributeCollection();
	private InventorySlots _slots = new InventorySlots();
	private AbilityCollection _abilityCollection = new AbilityCollection();

	private UnitData _u1 = new UnitData();

	[SetUp]
	public void TestLoadData() {

		// Setup attributes
		Attribute a1 = new Attribute (AttributeEnums.AttributeType.EXPERIENCE, "EXP", "EXP", "", 50, 0, 100);
		Attribute a2 = new Attribute (AttributeEnums.AttributeType.LEVEL, "LVL", "LVL", "", 5, 1, 10);
		_ac.Add (a1);
		_ac.Add (a2);

		// Setup inventory slots
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", _ac, InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg", "swordSFX");
		Item i2 = new Item (1, Item.ItemType.ARMOR, "armor", "aermor description", "armor", _ac, InventorySlots.SlotType.BODY, Item.ItemTier.TIER_3, "armor.jpg", "swordSFX");
		_slots.Add (i1);
		_slots.Add (i2);

		// Setup abiltiies
		Ability ability1 = new Ability(0, Ability.AbilityType.ATTACK, "attack", "attack_description", "attackTooltop", "attackIconPath", "attackVfxPath", 0, 0, null);
		Ability ability2 = new Ability(1, Ability.AbilityType.LAST_RESORT, "lr", "lrDescription", "lrTooltop", "lrIconPath", "lrVfxPath", 0, 0, null);
		_abilityCollection.Add (ability1);
		_abilityCollection.Add (ability2);

		// Setup units
		_u1.FirstName = "Leonard";
		_u1.LastName = "Bedner";
		_u1.AttributeCollection = _ac;
		_u1.Class = "Developer";
		_u1.InventorySlots = _slots;
		_u1.AbilityCollection = _abilityCollection;
	}

	[Test]
	public void TestDeepCopy() {

		// Make sure shallow copies are the same
		UnitData shallowCopy = _u1;
		Assert.AreSame (_u1, shallowCopy);

		UnitData deepCopy = _u1.DeepCopy ();
		Assert.AreNotSame (_u1, deepCopy);
	}
}