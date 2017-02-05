using UnityEngine;
using System.Collections;

public class ItemLoader {

	public static Inventory GetLoadedItems() {

		Inventory inventory = new Inventory ();

		// Sword of Galladoran
		AddNewItem(inventory, 0, Item.ItemType.WEAPON, "Sword of Galladoran", "Forged during the ancient Kilveran days, this sword has been in the Galladoran family for 4000 years", "Sword of Galladoran", AttributeLoader.GetSwordOfGalladoranAttributes (), "Prefabs/Icons/Items/Weapons/Swords/sword_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5);

		return inventory;
	}

	public static Inventory GetSinteresItems() {
		Inventory inventory = new Inventory ();
		AddNewItem(inventory, 1, Item.ItemType.WEAPON, "Daishan Dagger", "All purpose dagger given to all Daishan assassin initiates", "Daishan Dagger", AttributeLoader.GetDaishanDaggerAttributes (), "Prefabs/Icons/Items/Weapons/Daggers/dagger_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		return inventory;
	}

	public static Inventory GetOrelleItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 2, Item.ItemType.WEAPON, "Dundalan Axe", "Two sided axe from Dundalas", "Dundalan Axe", AttributeLoader.GetOrelleWeaponAttributes (), "Prefabs/Icons/Items/Weapons/Axes/axe_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		return inventory;
	}

	public static Inventory GetJarlItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 3, Item.ItemType.WEAPON, "Wand of Power", "Wand discovered in Elloquince", "Wand of Power", AttributeLoader.GetJarlWeaponAttributes (), "Prefabs/Icons/Items/Weapons/Books/book_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		return inventory;
	}

	public static Inventory GetMuckItems() {
		Inventory inventory = new Inventory ();
		AddNewItem (inventory, 4, Item.ItemType.MONSTER_ATTACK, "Muck", "Stick muck", "Muck", AttributeLoader.GetMuckWeaponAttributes (), "", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_1);
		return inventory;
	}

	public static Inventory GetMockInventoryItems() {
		Inventory inventory = new Inventory();

		AddNewItem(inventory, 0, Item.ItemType.WEAPON, "Sword of Galladoran", "Forged during the ancient Kilveran days, this sword has been in the Galladoran family for 4000 years.", "Sword of Galladoran", AttributeLoader.GetSwordOfGalladoranAttributes (), "Prefabs/Icons/Items/Weapons/Swords/sword_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5);
		AddNewItem(inventory, 1, Item.ItemType.WEAPON, "Daishan Dagger", "All purpose dagger given to all Daishan assassin initiates", "Daishan Dagger", AttributeLoader.GetDaishanDaggerAttributes (), "Prefabs/Icons/Items/Weapons/Daggers/dagger_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 2, Item.ItemType.WEAPON, "Dundalan Axe", "Two sided axe from Dundalas", "Dundalan Axe", AttributeLoader.GetOrelleWeaponAttributes (), "Prefabs/Icons/Items/Weapons/Axes/axe_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 3, Item.ItemType.WEAPON, "Wand of Power", "Wand discovered in Elloquince", "Wand of Power", AttributeLoader.GetJarlWeaponAttributes (), "Prefabs/Icons/Items/Weapons/Books/book_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 4, Item.ItemType.ARMOR, "Dundalan Chest Plate", "Armor given to all Knights of Dundalas, enchanted by Stramadonians to ward off magic attacks.", "Dundalan Armor", AttributeLoader.GetDundalanChestAttributes(), "Prefabs/Icons/Items/Armor/Chest/Heavy/chest_1", InventorySlots.SlotType.BODY, Item.ItemTier.TIER_4);
		AddNewItem (inventory, 5, Item.ItemType.ARMOR, "Dundalan Gauntlets", "Armor given to all Knights of Dundalas, enchanted by Stramadonians to ward off magic attacks.", "Dundalan Gauntlets", AttributeLoader.GetDundalanArmsAttributes(), "Prefabs/Icons/Items/Armor/Arms/Heavy/gauntlets_1", InventorySlots.SlotType.HANDS, Item.ItemTier.TIER_4);
		AddNewItem (inventory, 6, Item.ItemType.ARMOR, "Dundalan Boots", "Armor given to all Knights of Dundalas, enchanted by Stramadonians to ward off magic attacks.", "Dundalan Boots", AttributeLoader.GetDundalanLegsAttributes(), "Prefabs/Icons/Items/Armor/Legs/Heavy/boots_1", InventorySlots.SlotType.FEET, Item.ItemTier.TIER_4);
		AddNewItem (inventory, 7, Item.ItemType.ARMOR, "Dundalan Shield", "Armor given to all Knights of Dundalas, enchanted by Stramadonians to ward off magic attacks.", "Dundalan Shield", AttributeLoader.GetDundalanShieldAttributes(), "Prefabs/Icons/Items/Armor/Shields/Heavy/shield_1", InventorySlots.SlotType.LEFT_HAND, Item.ItemTier.TIER_4);
		AddNewItem (inventory, 8, Item.ItemType.CONSUMABLE, "Fleetness Tonic", "Increases the speed of the unit.", "Fleetness Tonic", new AttributeCollection(), "Prefabs/Icons/Items/Tonics/fleetness_tonic", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 9, Item.ItemType.ARMOR, "Justice Prevails Chest Plate", "Initiate armor forged by the great Naylorz himself.", "Justice Prevails Chest Plate", AttributeLoader.GetJusticePrevailsChestAttributes(), "Prefabs/Icons/Items/Armor/Chest/Heavy/chest_2", InventorySlots.SlotType.BODY, Item.ItemTier.TIER_3);
		AddNewItem (inventory, 10, Item.ItemType.ARMOR, "Justice Prevails Gauntlets", "Initiate armor forged by the great Naylorz himself.", "Justice Prevails Gauntlets", AttributeLoader.GetJusticePrevailsArmsAttributes(), "Prefabs/Icons/Items/Armor/Arms/Heavy/gauntlets_2", InventorySlots.SlotType.HANDS, Item.ItemTier.TIER_3);
		AddNewItem (inventory, 11, Item.ItemType.ARMOR, "Justice Prevails Boots", "Initiate armor forged by the great Naylorz himself.", "Justice Prevails Boots", AttributeLoader.GetJusticePrevailsLegsAttributes(), "Prefabs/Icons/Items/Armor/Legs/Heavy/boots_2", InventorySlots.SlotType.FEET, Item.ItemTier.TIER_3);
		AddNewItem (inventory, 12, Item.ItemType.ARMOR, "Justice Prevails Shield", "Initiate armor forged by the great Naylorz himself.", "Justice Prevails Shield", AttributeLoader.GetJusticePrevailsShieldAttributes(), "Prefabs/Icons/Items/Armor/Shields/Wooden/shield_1", InventorySlots.SlotType.LEFT_HAND, Item.ItemTier.TIER_3);
		AddNewItem (inventory, 13, Item.ItemType.CONSUMABLE, "Tonic Of Rebirth", "Drinking this tonic will bring a party member back to life.", "Tonic of Rebirth", new AttributeCollection(), "Prefabs/Icons/Items/Tonics/tonic_of_rebirth", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_4);
		AddNewItem (inventory, 14, Item.ItemType.CONSUMABLE, "Minor Tonic of Robustness", "Replenishes some health.", "Minor Tonic of Robustness", new AttributeCollection(), "Prefabs/Icons/Items/Tonics/robustness_tonic_1", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_1);
		AddNewItem (inventory, 15, Item.ItemType.ARMOR, "Left Ring of Galladoran", "Forged during the ancient Kilveran days, this item has been in the Galladoran family for 4000 years.", "Left Ring of Galladoran", AttributeLoader.GetLeftRingOfGalladoranAttributes(), "Prefabs/Icons/Items/Accessories/Rings/left_ring_of_galladoran", InventorySlots.SlotType.RING_1, Item.ItemTier.TIER_5);
		AddNewItem (inventory, 16, Item.ItemType.ARMOR, "Right Ring of Galladoran", "Forged during the ancient Kilveran days, this item has been in the Galladoran family for 4000 years.", "Right Ring of Galladoran", AttributeLoader.GetRightRingOfGalladoranAttributes(), "Prefabs/Icons/Items/Accessories/Rings/right_ring_of_galladoran", InventorySlots.SlotType.RING_2, Item.ItemTier.TIER_5);
		AddNewItem (inventory, 17, Item.ItemType.WEAPON, "Boundary Warden Longbow", "Longbow used by all new Boundary Warden recruits.", "Boundary Warden Longbow", AttributeLoader.GetLongBowAttributes(), "Prefabs/Icons/Items/Weapons/Ranged/Bows/bow_1", InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_1);
		AddNewItem (inventory, 18, Item.ItemType.CONSUMABLE, "Iron Arrow", "Shoot with bow arrow to pierce target.", "Iron Arrow", new AttributeCollection(), "Prefabs/Icons/Items/Weapons/Ranged/Arrows/arrows_1", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_1);
		AddNewItem (inventory, 19, Item.ItemType.CONSUMABLE, "Permafrost Arrow", "Shoot with bow to temporarily freeze a target.", "Permafrost Arrow", new AttributeCollection(), "Prefabs/Icons/Items/Weapons/Ranged/Arrows/ice_arrows_1", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 20, Item.ItemType.CONSUMABLE, "Inferno Arrow", "Shoot with bow to inflict fire damage over time on a target.", "Inferno Arrow", new AttributeCollection(), "Prefabs/Icons/Items/Weapons/Ranged/Arrows/inferno_arrows_1", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_2);
		AddNewItem (inventory, 21, Item.ItemType.CONSUMABLE, "Charging Crystal", "Hold while charging a spell to increase the potency of the spell.", "Charging Crystal", new AttributeCollection(), "Prefabs/Icons/Items/Crystals/crystal_1", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_3);
		AddNewItem (inventory, 22, Item.ItemType.CONSUMABLE, "Summoning Crystal", "Use to summon a creature from the Hollows.", "Summoning Crystal", new AttributeCollection(), "Prefabs/Icons/Items/Crystals/crystal_2", InventorySlots.SlotType.CONSUMABLE, Item.ItemTier.TIER_4);

		return inventory;
	}

	private static void AddNewItem(Inventory inventory, int id, Item.ItemType type, string name, string description, string toolTip, AttributeCollection attributes, string iconPath, InventorySlots.SlotType slotType, Item.ItemTier tier) {
		Item item = new Item (id, type, name, description, toolTip, attributes, slotType, tier);
		item.Icon = Resources.Load<Sprite> (iconPath);
		inventory.Add (item);
	}
}