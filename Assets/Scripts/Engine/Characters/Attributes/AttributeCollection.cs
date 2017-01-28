using UnityEngine;
using System.Collections.Generic;
using System.Text;
using RoseOfEternity;

public class AttributeCollection {

	/// <summary>
	/// The attributes.
	/// </summary>
	public Dictionary<AttributeEnums.AttributeType, Attribute> _attributes = new Dictionary<AttributeEnums.AttributeType, Attribute>();

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
	/// Get the specified type.
	/// </summary>
	/// <param name="type">Type.</param>
	public Attribute Get(AttributeEnums.AttributeType type) {
		if (_attributes.ContainsKey (type))
			return _attributes [type];
		return null;
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
}