using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class InventorySlotsTest {

	private Item _i1;
	private Inventory _inventory;
	private InventorySlots _slots;

	[SetUp]
	public void TestSetup() {
		_i1 = new Item (0, Item.ItemType.ARMOR, "armor", "it's armor", "armor", new AttributeCollection (), InventorySlots.SlotType.BODY, Item.ItemTier.TIER_5);
		_slots = new InventorySlots ();
	}

	[Test]
	public void TestAddAndGet() {

		_slots.Add (_i1);

		// Make sure the item is added to the slot
		Assert.AreEqual(_i1, _slots.Get(InventorySlots.SlotType.BODY));
	}

	[Test]
	public void TestRemove() {

		_slots.Add (_i1);

		Assert.NotNull (_slots.Get (InventorySlots.SlotType.BODY));

		_slots.Remove (InventorySlots.SlotType.BODY);

		Assert.Null (_slots.Get (InventorySlots.SlotType.BODY));
	}
}