using UnityEngine;
using System.Collections.Generic;
using System.Text;

using EternalEngine;

public class AttributeCollection {

	/// <summary>
	/// The attributes.
	/// </summary>
	private Dictionary<AttributeEnums.AttributeType, Attribute> _attributes = new Dictionary<AttributeEnums.AttributeType, Attribute>();

	/// <summary>
	/// Initializes a new instance of the <see cref="AttributeCollection"/> class.
	/// </summary>
	public AttributeCollection() {}

	/// <summary>
	/// Takes in a key/value pair of attribute/values, uses this to pull full Attributes from global collection, and creates new local collection.
	/// </summary>
	/// <returns>The from global collection.</returns>
	/// <param name="attributeDictionary">Attribute dictionary.</param>
	/// <param name="globalCollection">Global collection.</param>
	/// <param name="newCollection">New collection.</param>
	public static AttributeCollection GetFromGlobalCollection(Dictionary<AttributeEnums.AttributeType, float> attributeDictionary, AttributeCollection globalCollection, AttributeCollection newCollection) {

		// Set attributes in their collection. If attribute already exists, set value,
		// else, grab from global collection and set new attribute and value
		foreach (var item in attributeDictionary) {
			AttributeEnums.AttributeType attributeType = item.Key;
			Attribute attribute;
			if (!newCollection.HasType (attributeType)) {
				attribute = globalCollection.Get (attributeType).DeepCopy ();
				newCollection.Add (attributeType, attribute);
			} 
			else
				attribute = newCollection.Get (attributeType);
			
			attribute.CurrentValue = item.Value;
		}
		return newCollection;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AttributeCollection"/> class.
	/// </summary>
	/// <param name="attributes">Attributes.</param>
	public AttributeCollection(Dictionary<AttributeEnums.AttributeType, Attribute> attributes) {
		_attributes = attributes;
	}

	/// <summary>
	/// Returns a deep copied instance.
	/// </summary>
	/// <returns>The copy.</returns>
	public AttributeCollection DeepCopy() {
		var targetAttributes = new Dictionary<AttributeEnums.AttributeType, Attribute>();
		foreach (var attribute in _attributes)
			targetAttributes.Add (attribute.Key, attribute.Value.DeepCopy());
		return new AttributeCollection (targetAttributes);
	}

	/// <summary>
	/// Add the specified attribute.
	/// </summary>
	/// <param name="attribute">Attribute.</param>
	public void Add(Attribute attribute) {
		Add (attribute.Type, attribute);
	}

	/// <summary>
	/// Add the specified type and attribute.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="attribute">Attribute.</param>
	public void Add(AttributeEnums.AttributeType type, Attribute attribute) {
		if (!_attributes.ContainsKey(type))
			_attributes.Add (type, attribute);
	}

	/// <summary>
	/// Get the specified type of attribute.
	/// </summary>
	/// <param name="type">Type.</param>
	public Attribute Get(AttributeEnums.AttributeType type) {
		if (_attributes.ContainsKey (type))
			return _attributes [type];
		return null;
	}

	/// <summary>
	/// Gets the attributes.
	/// </summary>
	/// <returns>The attributes.</returns>
	public Dictionary<AttributeEnums.AttributeType, Attribute> GetAttributes() {
		return _attributes;
	}

	/// <summary>
	/// Returns attribute count.
	/// </summary>
	public int Count() {
		return _attributes.Count;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="AttributeCollection"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="AttributeCollection"/>.</returns>
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (Attribute attribute in _attributes.Values)
			sb.Append (attribute.ToString ());
		return sb.ToString ();
	}

	/// <summary>
	/// Determines whether this instance has the specified type.
	/// </summary>
	/// <returns><c>true</c> if this instance has the specified type; otherwise, <c>false</c>.</returns>
	/// <param name="type">Type.</param>
	private bool HasType(AttributeEnums.AttributeType type) {
		return _attributes.ContainsKey (type);
	}
}