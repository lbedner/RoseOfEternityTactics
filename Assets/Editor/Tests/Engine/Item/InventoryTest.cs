using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class InventoryTest {

	private Item _i1;
	private Item _i2;
	private Item _i3;
	private Inventory _inventory;

	[SetUp]
	public void Setup() {
		_i1 = new Item (0, Item.ItemType.ARMOR, "armor", "it's armor", "armor", new AttributeCollection ());
		_i2 = new Item (0, Item.ItemType.WEAPON, "weapon", "it's a weapon", "weapon", new AttributeCollection ());
		_i3 = new Item (0, Item.ItemType.WEAPON, "weapon", "it's a weapon", "weapon", new AttributeCollection ());
	}

	[Test]
	public void TestAddAndCount() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_i2);

		Assert.AreEqual (2, _inventory.Count ());
	}

	[Test]
	public void TestRemoveAndCount() {
		_inventory = new Inventory ();
		_inventory.Add (_i1);
		_inventory.Add (_i2);
		_inventory.Remove (_i1);
		_inventory.Remove (_i2);

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
}