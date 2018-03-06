public class AnimationSpeedEffect : Effect {

	private float _oldSpeed;
	private float _newSpeed;

	/// <summary>
	/// Initializes a new instance of the <see cref="AnimationSpeedEffect"/> class.
	/// </summary>
	/// <param name="speed">Speed.</param>
	/// <param name="turns">Turns.</param>
	public AnimationSpeedEffect(float speed, int turns = 0, EffectType incomingEffectType = EffectType.TEMPORARY) {
		_newSpeed = speed;
		_turns = turns;
		effectType = incomingEffectType;
	}

	/// <summary>
	/// Applies the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void ApplyEffect (Unit unit) {
		UnitAnimationController animationController = unit.GetAnimationController ();

		_oldSpeed = animationController.GetSpeed ();
		animationController.SetSpeed (_newSpeed);
		base.ApplyEffect (unit);
	}

	/// <summary>
	/// Removes the effect.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public override void RemoveEffect(Unit unit) {
		unit.GetAnimationController ().SetSpeed (_oldSpeed);
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