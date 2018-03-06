using UnityEngine;

public class UnitColorEffect : Effect {

	private Color _color;

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitColorEffect"/> class.
	/// </summary>
	/// <param name="color">Color.</param>
	/// <param name="turns">Turns.</param>
	public UnitColorEffect(Color color, int turns, EffectType incomingEffectType = EffectType.TEMPORARY) {
		_color = color;
		_turns = turns;
		effectType = incomingEffectType;
	}

	/// <summary>
	/// Applies the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void ApplyEffect(Unit unit) {
		unit.AddUnitColorEffect (this);
		base.ApplyEffect (unit);
	}

	/// <summary>
	/// Removes the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void RemoveEffect(Unit unit) {
		unit.StopChangeColorCoroutines ();
		unit.RemoveUnitColorEffect (this);
	}

	/// <summary>
	/// Gets the color.
	/// </summary>
	/// <returns>The color.</returns>
	public override Color GetColor() {
		return _color;
	}

	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <returns>The type.</returns>
	public override EffectType GetEffectType() {
		return effectType;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="AnimationSpeedEffect"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="AnimationSpeedEffect"/>.</returns>
	public override string ToString () {
		return "";
	}
}