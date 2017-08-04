using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class InventoryTest {

	private Item _i1;
	private Item _i2;
	private Item _i3;
	private Item _nullItem;
	private Inventory _inventory;

	[SetUp]
	public void Setup() {
		_i1 = new Item (0, Item.ItemType.ARMOR, "armor", "it's armor", "armor", new AttributeCollection (), InventorySlots.SlotType.BODY, Item.ItemTier.TIER_5, "armor.png", "swordSFX");
		_i2 = new Item (0, Item.ItemType.WEAPON, "weapon", "it's a weapon", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg", "swordSFX");
		_i3 = new Item (0, Item.ItemType.WEAPON, "weapon", "it's a weapon", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg", "swordSFX");
		_nullItem = null;
	}

	[Test]
	public void TestAddAndCount() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_i2);
		_inventory.Add (_nullItem);

		Assert.AreEqual (2, _inventory.Count ());
	}

	[Test]
	public void TestRemoveAndCount() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_i2);
		_inventory.Add (_nullItem);
		_inventory.Remove (_i1);
		_inventory.Remove (_i2);
		_inventory.Remove (_nullItem);

		Assert.AreEqual (0, _inventory.Count ());
	}

	[Test]
	public void TestGetFirstItemByName() {
		_inventory = new Inventory ();

		string itemName = "armor";

		// Check for an item that doesn't exist
		Assert.Null(_inventory.GetFirstItemByName(itemName));

		// Add item, then check to make sure it's there
		_inventory.Add(_i1);
		Assert.AreEqual (_i1, _inventory.GetFirstItemByName (itemName));
	}

	[Test]
	public void TestGetItemsByName() {
		_inventory = new Inventory ();

		string itemName = "weapon";

		Assert.AreEqual (0, _inventory.GetItemsByName (itemName).Count);

		_inventory.Add (_i2);
		_inventory.Add (_i3);

		List<Item> items = _inventory.GetItemsByName (itemName);

		Assert.AreEqual (2, items.Count);
		Assert.AreEqual (_i2, items [0]);
		Assert.AreEqual (_i3, items [1]);
	}

	[Test]
	public void TestGet() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_nullItem);

		Assert.AreEqual (_i1, _inventory.Get (0));
		Assert.AreEqual (_nullItem, _inventory.Get (1));
	}

	[Test]
	public void TestUpsert() {
		_inventory = new Inventory ();

		// Inventory is currently empty, test that upserting to the first index (which doesn't exist) works
		_inventory.Upsert (_i1, 0);
		Assert.AreEqual (_i1, _inventory.Get (0));

		// Test inserting to an index that is clearly bigger than inventory
		_inventory.Upsert(_i2, 100);
		Assert.AreEqual (_i2, _inventory.Get (1));

		// Update first item in inventory
		_inventory.Upsert(_i3, 0);
		Assert.AreEqual (_i3, _inventory.Get (0));
	}

	[Test]
	public void TestDeepCopy() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_i2);
		_inventory.Add (_i3);

		// Test that shallow copies are the same
		Inventory shallowCopy = _inventory;
		Assert.AreSame (_inventory, shallowCopy);

		foreach (var shallowItem in shallowCopy.Items)
			Assert.AreSame (_inventory.Get (_inventory.Items.IndexOf (shallowItem)), shallowItem);

		// Test that deep copies are different
		Inventory deepCopy = _inventory.DeepCopy();
		Assert.AreNotSame (_inventory, deepCopy);

		for (int index = 0; index < _inventory.Count(); index++)
			Assert.AreNotSame (_inventory.Get (index), deepCopy.Get (index));
	}
}