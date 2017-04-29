using UnityEngine;
using NUnit.Framework;
using RoseOfEternity;
using System.Collections;

[TestFixture]
public class ItemTest {

	[Test]
	public void TestGetAttributeSuccess() {

		AttributeEnums.AttributeType type = AttributeEnums.AttributeType.ABILITY_POINTS;

		Attribute a1 = new Attribute (type, "test_get_attribute", "test", "test", 0.0f, 0.0f, 10.0f);
		AttributeCollection attributes = new AttributeCollection ();
		attributes.Add (type, a1);

		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", attributes, InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg");

		Assert.NotNull (i1.GetAttribute (type));
	}

	[Test]
	public void TestGetAttributesFailiure() {
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_1, "weapon.jpg");

		Assert.Null(i1.GetAttribute(AttributeEnums.AttributeType.ABILITY_POINTS));
	}

	[Test]
	public void TestGetTierColor() {
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_1, "weapon.jpg");
		Assert.AreEqual (i1.Tier1Color, i1.TierColor);

		i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2, "weapon.jpg");
		Assert.AreEqual (i1.Tier2Color, i1.TierColor);

		i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_3, "weapon.jpg");
		Assert.AreEqual (i1.Tier3Color, i1.TierColor);

		i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_4, "weapon.jpg");
		Assert.AreEqual (i1.Tier4Color, i1.TierColor);

		i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg");
		Assert.AreEqual (i1.Tier5Color, i1.TierColor);
	}

	[Test]
	public void TestGetTierName() {
		Item i = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_1, "weapon.jpg");
		Assert.AreEqual (Item.TIER_1_NAME, i.TierName);

		i = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_2, "weapon.jpg");
		Assert.AreEqual (Item.TIER_2_NAME, i.TierName);

		i = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_3, "weapon.jpg");
		Assert.AreEqual (Item.TIER_3_NAME, i.TierName);

		i = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_4, "weapon.jpg");
		Assert.AreEqual (Item.TIER_4_NAME, i.TierName);

		i = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection (), InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg");
		Assert.AreEqual (Item.TIER_5_NAME, i.TierName);
	}

	[Test]
	public void TestDeepCopy() {
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", null, InventorySlots.SlotType.RIGHT_HAND, Item.ItemTier.TIER_5, "weapon.jpg");
		Item shallowCopy = i1;

		Assert.AreSame (i1, shallowCopy);
		Assert.AreNotSame (i1, i1.DeepCopy());
	}
}