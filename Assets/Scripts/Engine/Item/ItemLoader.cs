using UnityEngine;
using System.Collections;

public class ItemLoader {

	public static Inventory GetLoadedItems() {

		Inventory inventory = new Inventory ();

		// Sword of Galladoran
		AddNewItem(inventory, 0, Item.ItemType.WEAPON, "Sword of Galladoran", "Forged during the ancient Kilveran days, this sword has been in the Galladoran family for 4000 years", "Sword of Galladoran", AttributeLoader.GetSwordOfGalladoranAttributes ());

		return inventory;
	}

	public static Inventory GetSinteresItems() {
		Inventory inventory = new Inventory ();
		AddNewItem(inventory, 1, Item.ItemType.WEAPON, "Daishan Dagger", "All purpose dagger given to all Daishan assassin initiates", "Daishan Dagger", AttributeLoader.GetDaishanDaggerAttributes ());
		return inventory;
	}

	public static Inventory GetOrelleItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 2, Item.ItemType.WEAPON, "Dundalan Axe", "Two sided axe from Dundalas", "Dundalan Axe", AttributeLoader.GetOrelleWeaponAttributes ());
		return inventory;
	}

	public static Inventory GetJarlItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 3, Item.ItemType.WEAPON, "Wand of Power", "Wand discovered in Elloquince", "Wand of Power", AttributeLoader.GetJarlWeaponAttributes ());
		return inventory;
	}

	public static Inventory GetMuckItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 4, Item.ItemType.MONSTER_ATTACK, "Muck", "Stick muck", "Muck", AttributeLoader.GetMuckWeaponAttributes ());
		return inventory;
	}

	private static void AddNewItem(Inventory inventory, int id, Item.ItemType type, string name, string description, string toolTip, AttributeCollection attributes) {
		Item item = new Item (id, type, name, description, toolTip, attributes);
		inventory.Add (item);
	}
}