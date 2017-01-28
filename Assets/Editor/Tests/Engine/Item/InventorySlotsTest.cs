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
		_i1 = new Item (0, Item.ItemType.ARMOR, "armor", "it's armor", "armor", new AttributeCollection ());
		_inventory = new Inventory ();

		_inventory.Add (_i1);
		_slots = new InventorySlots ();
	}

	[Test]
	public void TestAddAndGet() {

		string itemName = "armor";

		// Make sure the inventory has the item
		Assert.AreEqual(_i1, _inventory.GetFirstItemByName(itemName));

		_slots.Add (InventorySlots.Slot.BODY, _inventory, _i1);

		// Make sure item is removed from the inventory
		Assert.Null(_inventory.GetFirstItemByName(itemName));

		// Make sure the item is added to the slot
		Assert.AreEqual(_i1, _slots.Get(InventorySlots.Slot.BODY));
	}

	[Test]
	public void TestRemove() {

		string itemName = "armor";

		_slots.Add (InventorySlots.Slot.BODY, _inventory, _i1);

		Assert.NotNull (_slots.Get (InventorySlots.Slot.BODY));

		_slots.Remove (InventorySlots.Slot.BODY, _inventory);

		Assert.Null (_slots.Get (InventorySlots.Slot.BODY));
		Assert.AreEqual (_i1, _inventory.GetFirstItemByName (itemName));
	}
}