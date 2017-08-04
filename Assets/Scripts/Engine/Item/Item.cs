using UnityEngine;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using EternalEngine;

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
	[JsonProperty] public int Id { get; private set; }
	[JsonProperty] public string Name { get; private set; }
	[JsonProperty] public string Description { get; private set; }
	[JsonProperty] public string ToolTip { get; private set; }
	[JsonProperty] public string IconPath { get; private set; }
	[JsonProperty] public string SoundPath { get; private set; }
	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))] public ItemType Type { get; private set; }
	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))] public InventorySlots.SlotType SlotType { get; private set; }
	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))] public ItemTier Tier { get; private set; }

	[JsonIgnore] public Sprite Icon { get; set; }
	[JsonIgnore] public AudioClip Sound { get; private set; }
	[JsonIgnore] public Color TierColor { get; private set; }
	[JsonIgnore] public string TierName { get; private set; }

	[JsonIgnore] public Color Tier1Color { get { return Color.grey; } }
	[JsonIgnore] public Color Tier2Color { get { return new Color32 (52, 97, 68, 255); } }
	[JsonIgnore] public Color Tier3Color { get { return new Color32 (66, 114, 163, 255); } }
	[JsonIgnore] public Color Tier4Color { get { return new Color32 (81, 49, 99, 255); } }
	[JsonIgnore] public Color Tier5Color { get { return new Color32 (196, 169, 58, 255); } }

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
	/// <param name="slot">Slot.</param>
	/// <param name="tier">Tier.</param>
	/// <param name="iconPath">Icon path.</param>
	public Item(int id, ItemType itemType, string name, string description, string toolTip, AttributeCollection attributeCollection, InventorySlots.SlotType slot, ItemTier tier, string iconPath, string soundPath) {
		Init (id, itemType, name, description, toolTip, attributeCollection, slot, tier, iconPath, soundPath);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Item"/> class.
	/// Only used when deserialializing the object, values from item.json.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="itemType">Item type.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="attributes">Attributes.</param>
	/// <param name="slot">Slot.</param>
	/// <param name="tier">Tier.</param>
	/// <param name="iconPath">Icon path.</param>
	/// <param name="soundPath">Sound path.</param>
	[JsonConstructor]
	private Item(int id, ItemType itemType, string name, string description, string toolTip, Dictionary<AttributeEnums.AttributeType, float> attributes, InventorySlots.SlotType slot, ItemTier tier, string iconPath, string soundPath) {
		Init (id, itemType, name, description, toolTip, null, slot, tier, iconPath, soundPath);

		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		if (_attributeCollection == null)
			_attributeCollection = new AttributeCollection ();
		_attributeCollection = AttributeCollection.GetFromGlobalCollection (attributes, globalAttributeCollection, _attributeCollection);
	}

	/// <summary>
	/// Returns a deep copied instance.
	/// </summary>
	/// <returns>The deep copied instance.</returns>
	public Item DeepCopy() {
		return new Item (Id, Type, Name, Description, ToolTip, _attributeCollection, SlotType, Tier, IconPath, SoundPath);
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
	/// Initializes a new instance of the <see cref="Item"/> class.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="itemType">Item type.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="attributeCollection">Attribute collection.</param>
	/// <param name="slot">Slot.</param>
	/// <param name="tier">Tier.</param>
	/// <param name="iconPath">Icon path.</param>
	private void Init(int id, ItemType itemType, string name, string description, string toolTip, AttributeCollection attributeCollection, InventorySlots.SlotType slot, ItemTier tier, string iconPath, string soundPath) {
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
		IconPath = iconPath;
		Icon = Resources.Load<Sprite> (IconPath);
		SoundPath = soundPath;
		Sound = Resources.Load<AudioClip> (SoundPath);
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