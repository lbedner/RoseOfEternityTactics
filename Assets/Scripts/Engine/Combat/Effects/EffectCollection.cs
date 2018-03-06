using System.Collections.Generic;
using System.Text;

public class EffectCollection {

	/// <summary>
	/// The effects.
	/// </summary>
	private List<Effect> _effects = new List<Effect>();

	/// <summary>
	/// Gets the effects.
	/// </summary>
	/// <returns>The effects.</returns>
	public List<Effect> GetEffects() {
		return _effects;
	}

	/// <summary>
	/// Add the specified effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void Add(Effect effect) {
		_effects.Add (effect);
	}

	/// <summary>
	/// Remove the specified effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void Remove(Effect effect) {
		_effects.Remove (effect);
	}

	/// <summary>
	/// Returns effect count.
	/// </summary>
	public int Count() {
		return _effects.Count;
	}
}