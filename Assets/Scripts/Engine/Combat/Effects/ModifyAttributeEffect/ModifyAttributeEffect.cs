using UnityEngine;

using EternalEngine;

/// <summary>
/// Modify attribute effect.
/// </summary>
public class ModifyAttributeEffect : Effect {

	private AttributeEnums.AttributeType _attributeType;
	private int _value;
	private int _incrementalValue;
	private Color _color;
	private string _vfxPath;

	/// <summary>
	/// Initializes a new instance of the <see cref="ModifyAttributeEffect"/> class.
	/// </summary>
	/// <param name="attributeType">Attribute type.</param>
	/// <param name="value">Value.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="incrementalValue">Incremental value.</param>
	/// <param name="color">Color.</param>
	/// <param name="effectType">Effect type.</param>
	public ModifyAttributeEffect(AttributeEnums.AttributeType attributeType, int value, int turns = 0, int incrementalValue = 0, Color color = default(Color), EffectType incomingEffectType = EffectType.INSTANT, string vfxPath = "") {
		_attributeType = attributeType;
		_value = value;
		_turns = turns;
		_incrementalValue = incrementalValue;
		_color = color;
		effectType = incomingEffectType;
		_vfxPath = vfxPath;
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <returns>The value.</returns>
	public int GetValue() {
		return _value;
	}

	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <returns>The type.</returns>
	public override EffectType GetEffectType() {
		return effectType;
	}

	/// <summary>
	/// Gets the VFX path.
	/// </summary>
	/// <returns>The VFX path.</returns>
	public override string GetVFXPath() {
		return _vfxPath;
	}

	/// <summary>
	/// Applies the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void ApplyEffect(Unit unit) {
		unit.GetAttribute (_attributeType).Increment (_value);
		if (_turns > 0)
			unit.AddModifyAttributeEffect (_attributeType, this);
		base.ApplyEffect (unit);
	}

	/// <summary>
	/// Applies the incremental effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void ApplyIncrementalEffect(Unit unit) {
		unit.GetAttribute (_attributeType).Increment (_incrementalValue);
	}

	/// <summary>
	/// Removes the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void RemoveEffect(Unit unit) {
		unit.GetAttribute (_attributeType).Decrement (_value);
		unit.RemoveModifyAttributeEffect (_attributeType);
	}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	public override string GetDisplayString () {
		return GetDisplayString (_value);
	}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	public override string GetIncrementalDisplayString () {
		return GetDisplayString (_incrementalValue);
	}

	/// <summary>
	/// Gets the color.
	/// </summary>
	/// <returns>The color.</returns>
	public override Color GetColor() {
		return _color;
	}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	/// <param name="value">Value.</param>
	protected virtual string GetDisplayString(int value) {
		if (value == 0)
			return "";

		string name = AttributeManager.Instance.GlobalAttributeCollection.Get (_attributeType).ShortName;
		string plusMinus = "+";
		if (value < 0)
			plusMinus = "";
		return string.Format ("{0}{1}{2}", name, plusMinus, value);
	}
}

/// <summary>
/// Heal effect.
/// </summary>
public class HealEffect : ModifyAttributeEffect {

	/// <summary>
	/// Initializes a new instance of the <see cref="HealEffect"/> class.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="incrementalValue">Incremental value.</param>
	/// <param name="color">Color.</param>
	public HealEffect (int value, int turns = 0, int incrementalValue = 0, Color color = default(Color), EffectType incomingEffectType = EffectType.INSTANT, string vfxPath = "") : base (AttributeEnums.AttributeType.HIT_POINTS, value, turns, incrementalValue, color, incomingEffectType, vfxPath) {}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	/// <param name="value">Value.</param>
	protected override string GetDisplayString(int value) {
		if (value == 0)
			return "";
		else
			return value.ToString ();
	}
}

/// <summary>
/// Damage effect.
/// </summary>
public class DamageEffect : ModifyAttributeEffect {

	/// <summary>
	/// Initializes a new instance of the <see cref="DamageEffect"/> class.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="incrementalValue">Incremental value.</param>
	/// <param name="color">Color.</param>
	public DamageEffect (int value, int turns = 0, int incrementalValue = 0, Color color = default(Color), EffectType incomingEffectType = EffectType.INSTANT, string vfxPath = ""): base (AttributeEnums.AttributeType.HIT_POINTS, value * -1, turns, incrementalValue * -1, color, incomingEffectType, vfxPath) {}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	/// <param name="value">Value.</param>
	protected override string GetDisplayString(int value) {
		if (value == 0)
			return "";
		else
			return (value * -1).ToString ();
	}
}

/// <summary>
/// Speed effect.
/// </summary>
public class SpeedEffect : ModifyAttributeEffect {

	/// <summary>
	/// Initializes a new instance of the <see cref="SpeedEffect"/> class.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="turns">Turns.</param>
	/// <param name="incrementalValue">Incremental value.</param>
	/// <param name="color">Color.</param>
	public SpeedEffect (int value, int turns = 0, int incrementalValue = 0, Color color = default(Color), EffectType incomingEffectType = EffectType.INSTANT, string vfxPath = "") : base (AttributeEnums.AttributeType.SPEED, value, turns, incrementalValue, color, incomingEffectType, vfxPath) {}
}