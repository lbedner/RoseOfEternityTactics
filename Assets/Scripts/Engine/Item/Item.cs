using UnityEngine;
using System.Collections;
using RoseOfEternity;

public class Item {

	public const string TIER_1_NAME = "Normal";
	public const string TIER_2_NAME = "Uncommon";
	public const string TIER_3_NAME = "Rare";
	public const string TIER_4_NAME = "Legendary";
	public const string TIER_5_NAME = "Exotic";

	public enum ItemType {
		WEAPON,
		ARMOR,
		CONSUMABLE,
		MONSTER_ATTACK,
	}

	public enum ItemTier {
		TIER_1,
		TIER_2,
		TIER_3,
		TIER_4,
		TIER_5,
	}

	// Properties
	public int Id { get; private set; }
	public ItemType Type { get; private set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string ToolTip { get; set; }
	public Sprite Icon { get; set; }
	public InventorySlots.SlotType SlotType { get; set; }
	public ItemTier Tier { get; private set; }
	public Color TierColor { get; private set; }
	public string TierName { get; private set; }

	public Color Tier1Color { get { return Color.grey; } }
	public Color Tier2Color { get { return new Color32 (52, 97, 68, 255); } }
	public Color Tier3Color { get { return new Color32 (66, 114, 163, 255); } }
	public Color Tier4Color { get { return new Color32 (81, 49, 99, 255); } }
	public Color Tier5Color { get { return new Color32 (196, 169, 58, 255); } }

	private AttributeCollection _attributeCollection;

	/// <summary>
	/// Initializes a new instance of the <see cref="Item"/> class.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="itemType">Item type.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="attributeCollection">Attribute collection.</param>
	public Item(int id, ItemType itemType, string name, string description, string toolTip, AttributeCollection attributeCollection, InventorySlots.SlotType slot, ItemTier tier) {
		Id = id;
		Type = itemType;
		Name = name;
		Description = description;
		ToolTip = toolTip;
		_attributeCollection = attributeCollection;
		SlotType = slot;
		Tier = tier;
		TierColor = GetTierColor ();
		TierName = GetTierName ();
	}

	/// <summary>
	/// Gets the attribute by type.
	/// </summary>
	/// <returns>The attribute.</returns>
	/// <param name="attributeType">Attribute type.</param>
	public Attribute GetAttribute(AttributeEnums.AttributeType attributeType) {
		return _attributeCollection.Get (attributeType);
	}

	/// <summary>
	/// Gets the attribute collection.
	/// </summary>
	/// <returns>The attribute collection.</returns>
	public AttributeCollection GetAttributeCollection() {
		return _attributeCollection;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Item"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Item"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Item: Id={0}, Type={1}, Name={2}, Description={3}, ToolTip={4}, Icon={5}, Slot={6}, Attributes={7}, Tier={8}, TierColor={9}, TierName={10}]", Id, Type, Name, Description, ToolTip, Icon, SlotType, _attributeCollection, Tier, TierColor, TierName);
	}

	/// <summary>
	/// Gets the color of the tier.
	/// </summary>
	/// <returns>The tier color.</returns>
	private Color GetTierColor() {
		switch (Tier) {
		case ItemTier.TIER_1:
			return Tier1Color;
		case ItemTier.TIER_2:
			return Tier2Color;
		case ItemTier.TIER_3:
			return Tier3Color;
		case ItemTier.TIER_4:
			return Tier4Color;
		case ItemTier.TIER_5:
			return Tier5Color;
		default:
			return Tier1Color;
		}
	}

	/// <summary>
	/// Gets the name of the tier.
	/// </summary>
	/// <returns>The tier name.</returns>
	private string GetTierName() {
		switch (Tier) {
		case ItemTier.TIER_1:
			return TIER_1_NAME;
		case ItemTier.TIER_2:
			return TIER_2_NAME;
		case ItemTier.TIER_3:
			return TIER_3_NAME;
		case ItemTier.TIER_4:
			return TIER_4_NAME;
		case ItemTier.TIER_5:
			return TIER_5_NAME;
		default:
			return TIER_1_NAME;
		}
	}		
}