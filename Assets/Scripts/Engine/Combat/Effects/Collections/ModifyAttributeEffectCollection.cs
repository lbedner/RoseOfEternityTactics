using System.Collections.Generic;
using System.Text;

using EternalEngine;

public class ModifyAttributeEffectCollection {

	/// <summary>
	/// The effects.
	/// </summary>
	private Dictionary<AttributeEnums.AttributeType ,ModifyAttributeEffect> _effects = new Dictionary<AttributeEnums.AttributeType, ModifyAttributeEffect>();

	/// <summary>
	/// Gets the effects.
	/// </summary>
	/// <returns>The effects.</returns>
	public Dictionary<AttributeEnums.AttributeType, ModifyAttributeEffect> GetEffects() {
		return _effects;
	}

	/// <summary>
	/// Get the specified attributeType.
	/// </summary>
	/// <param name="attributeType">Attribute type.</param>
	public ModifyAttributeEffect Get(AttributeEnums.AttributeType attributeType) {
		if (_effects.ContainsKey (attributeType))
			return _effects [attributeType];
		return null;
	}

	/// <summary>
	/// Add the specified effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void Add(AttributeEnums.AttributeType attributeType, ModifyAttributeEffect effect) {
		_effects.Add (attributeType, effect);
	}

	/// <summary>
	/// Remove the specified effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void Remove(AttributeEnums.AttributeType attributeType) {
		if (_effects.ContainsKey (attributeType))
			_effects.Remove (attributeType);
	}

	/// <summary>
	/// Returns effect count.
	/// </summary>
	public int Count() {
		return _effects.Count;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="EffectCollection"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="EffectCollection"/>.</returns>
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (var effect in _effects.Values)
			sb.Append (effect.GetDisplayString());
		return sb.ToString ();
	}
}