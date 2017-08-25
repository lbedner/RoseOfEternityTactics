public class Effect
{
	/// <summary>
	/// Effect delegate.
	/// </summary>
	public delegate void EffectDelegate();

	/// <summary>
	/// Heal effect
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="value">Value.</param>
	public static void EffectHeal(Unit unit, int value) {
		unit.GetHitPointsAttribute ().Increment (value);
	}

	/// <summary>
	/// Damage effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="value">Value.</param>
	public static void EffectDamage(Unit unit, int value) {
		unit.GetHitPointsAttribute ().Decrement (value);
	}

	/// <summary>
	/// Speed effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="value">Value.</param>
	/// <param name="turns">Turns.</param>
	public static void EffectSpeed(Unit unit, int value, int turns) {
		unit.GetSpeedAttribute ().Increment (value);
	}
}