using UnityEngine;
using NUnit.Framework;
using RoseOfEternity;

[TestFixture]
public class UnitDataTest {

	private AttributeCollection _ac = new AttributeCollection();
	private InventorySlots _slots = new InventorySlots();

	private UnitData _u1 = new UnitData();

	[SetUp]
	public void TestLoadData() {

		// Setup attributes
		Attribute a1 = new Attribute (AttributeEnums.AttributeType.EXPERIENCE, "EXP", "EXP", "", 50, 0, 100);
		Attribute a2 = new Attribute (AttributeEnums.AttributeType.LEVEL, "LVL", "LVL", "", 5, 1, 10);
		_ac.Add (a1);
		_ac.Add (a2);

		// Setup inventory slots
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", _ac, InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg");
		Item i2 = new Item (1, Item.ItemType.ARMOR, "armor", "aermor description", "armor", _ac, InventorySlots.SlotType.BODY, Item.ItemTier.TIER_3, "armor.jpg");
		_slots.Add (i1);
		_slots.Add (i2);

		// Setup units
		_u1.FirstName = "Leonard";
		_u1.LastName = "Bedner";
		_u1.AttributeCollection = _ac;
		_u1.Class = "Developer";
		_u1.InventorySlots = _slots;
	}

	[Test]
	public void TestAttributes() {
		var attributes = _u1.Attributes;
		Assert.AreEqual (50, attributes [AttributeEnums.AttributeType.EXPERIENCE]);
		Assert.AreEqual (5, attributes [AttributeEnums.AttributeType.LEVEL]);
	}

	[Test]
	public void TestInventory() {
		var inventory = _u1.Inventory;
		Assert.AreEqual (0, inventory [InventorySlots.SlotType.RIGHT_HAND]);
		Assert.AreEqual (1, inventory [InventorySlots.SlotType.BODY]);
	}
}