using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using EternalEngine;

public class Ability {

	public enum AbilityType {
		ATTACK,
		MAGIC,
		TALENT,
		LAST_RESORT,
		UNISON_ABILITY,
		BONDS_OF_BATTLE,
	}

	[JsonProperty] public int Id { get; private set; }
	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))] public AbilityType Type { get; private set; }
	[JsonProperty] public string Name { get; private set; }
	[JsonProperty] public string Description { get; private set; }
	[JsonProperty] public string ToolTip { get; private set; }
	[JsonProperty] public string IconPath { get; private set; }
	[JsonProperty] public string VFXPath { get; private set; }
	[JsonProperty] public int Cost { get; private set; }
	[JsonProperty] public int Turns { get; private set; }

	private AttributeCollection _attributeCollection;

	/// <summary>
	/// Initializes a new instance of the <see cref="Ability"/> class.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="type">Ability Type.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="iconPath">Icon path.</param>
	/// <param name="vfxpPath">VFX path.</param>
	/// <param name="cost">Cost.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="attributeCollection">Attribute collection.</param>
	public Ability(int id, AbilityType type, string name, string description, string toolTip, string iconPath, string vfxPath, int cost, int turns, AttributeCollection attributeCollection) {
		Id = id;
		Type = type;
		Name = name;
		Description = description;
		ToolTip = toolTip;
		IconPath = iconPath;
		VFXPath = vfxPath;
		Cost = cost;
		Turns = turns;
		_attributeCollection = attributeCollection;
	}
		
	/// <summary>
	/// Initializes a new instance of the <see cref="Ability"/> class.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="type">Type.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="toolTip">Tool tip.</param>
	/// <param name="iconPath">Icon path.</param>
	/// <param name="vfxpPath">VFX path.</param>
	/// <param name="cost">Cost.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="attributes">Attributes.</param>
	[JsonConstructor]
	private Ability(int id, AbilityType type, string name, string description, string toolTip, string iconPath, string vfxPath, int cost, int turns, Dictionary<AttributeEnums.AttributeType, float> attributes) {
		Id = id;
		Type = type;
		Name = name;
		Description = description;
		ToolTip = toolTip;
		IconPath = iconPath;
		VFXPath = vfxPath;
		Cost = cost;
		Turns = turns;

		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		if (_attributeCollection == null)
			_attributeCollection = new AttributeCollection ();
		_attributeCollection = AttributeCollection.GetFromGlobalCollection (attributes, globalAttributeCollection, _attributeCollection);
	}

	/// <summary>
	/// Returns a deep copied instance.
	/// </summary>
	/// <returns>The deep copied instance.</returns>
	public Ability DeepCopy() {
		return new Ability (Id, Type, Name, Description, ToolTip, IconPath, VFXPath, Cost, Turns, _attributeCollection);
	}

	/// <summary>
	/// Gets the attribute collection.
	/// </summary>
	/// <returns>The attribute collection.</returns>
	public AttributeCollection GetAttributeCollection() {
		return _attributeCollection;
	}

	/// <summary>
	/// Gets the AOE range.
	/// </summary>
	/// <returns>The AOE range.</returns>
	public int GetAOERange() {
		return (int)GetAttributeValue (AttributeEnums.AttributeType.AOE_RANGE);
	}

	/// <summary>
	/// Gets the range.
	/// </summary>
	/// <returns>The range.</returns>
	public int GetRange() {
		return (int)GetAttributeValue (AttributeEnums.AttributeType.RANGE);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Ability"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Ability"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Ability: Id={0}, Type={1}, Name={2}, Description={3}, ToolTip={4}, IconPath={5}, VFXPath={6}, Cost={7}, Turns={8}, Attributes={8}]", Id, Type, Name, Description, ToolTip, IconPath, VFXPath, Cost, Turns, _attributeCollection);
	}

	/// <summary>
	/// Gets the attribute value.
	/// </summary>
	/// <returns>The attribute value.</returns>
	/// <param name="type">Type.</param>
	private float GetAttributeValue(AttributeEnums.AttributeType type) {
		float value = 0.0f;
		Attribute attribute = _attributeCollection.Get (type);
		if (attribute != null)
			value = attribute.CurrentValue;
		return value;
	}
}