using UnityEngine;

/// <summary>
/// Effect type.
/// </summary>
public enum EffectType {
	INSTANT,
	OVER_TIME,
	TEMPORARY,
}

/// <summary>
/// Effect.
/// </summary>
public class Effect
{

	protected int _turns;
	protected EffectType effectType;

	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <returns>The type.</returns>
	public virtual EffectType GetEffectType() { return EffectType.INSTANT; }

	/// <summary>
	/// Applies the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public virtual void ApplyEffect (Unit unit) {

		// If this is a temporary effect, add it to the unit
		if (_turns > 0)
			unit.AddEffect (this);
	}

	/// <summary>
	/// Apples the incremental effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public virtual void ApplyIncrementalEffect(Unit unit) {}

	/// <summary>
	/// Removes the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public virtual void RemoveEffect (Unit unit) {}

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	public virtual string GetDisplayString() { return ""; }

	/// <summary>
	/// Gets the display string.
	/// </summary>
	/// <returns>The display string.</returns>
	public virtual string GetIncrementalDisplayString() { return ""; }

	/// <summary>
	/// Gets the color.
	/// </summary>
	/// <returns>The color.</returns>
	public virtual Color GetColor() { return new Color(); }

	/// <summary>
	/// Gets the VFX path.
	/// </summary>
	/// <returns>The VFX path.</returns>
	public virtual string GetVFXPath() { return ""; }

	/// <summary>
	/// Decrements the turn.
	/// </summary>
	/// <returns>The turn.</returns>
	/// <param name="unit">Unit.</param>
	public virtual Effect DecrementTurn (Unit unit) {
		_turns--;
		if (_turns <= 0) {
			RemoveEffect (unit);
			return this;
		}
		return null;
	}
}