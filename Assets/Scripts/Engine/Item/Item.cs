using UnityEngine;
using System.Collections;
using RoseOfEternity;

public class Item {

	public enum ItemType {
		WEAPON,
		ARMOR,
		CONSUMABLE,
		MONSTER_ATTACK,
	}

	// Properties
	public int Id { get; private set; }
	public ItemType Type { get; private set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string ToolTip { get; set; }
	public Sprite Icon { get; set; }

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
	public Item(int id, ItemType itemType, string name, string description, string toolTip, AttributeCollection attributeCollection) {
		Id = id;
		Type = itemType;
		Name = name;
		Description = description;
		ToolTip = toolTip;
		_attributeCollection = attributeCollection;
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
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Item"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Item"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Item: Id={0}, Type={1}, Name={2}, Description={3}, ToolTip={4}, Icon={5}], Attributes={6}]", Id, Type, Name, Description, ToolTip, Icon, _attributeCollection);
	}
}