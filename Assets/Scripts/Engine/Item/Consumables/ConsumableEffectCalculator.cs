using UnityEngine;
using System;
using System.Collections.Generic;

public class ConsumableEffectCalculator
{
	/// <summary>
	/// Gets the consumable effects.
	/// </summary>
	/// <returns>The consumable effects.</returns>
	/// <param name="item">Item.</param>
	public static List<Effect> GetConsumableEffects(Item item) {
		List<Effect> effects = new List<Effect>();
		switch (item.Id) {
		case ConsumableConstants.TONIC_OF_ROBUSTNESS_1:
			effects.Add (new HealEffect (value: 10, color: Color.green));
			break;
		case ConsumableConstants.FLEETNESS_TONIC:
			int turns = 2;
			effects.Add (new SpeedEffect (value: 1, turns: turns, color: Color.red, incomingEffectType: EffectType.TEMPORARY));
			effects.Add (new AnimationSpeedEffect(speed: 3, turns: turns));
			effects.Add (new UnitColorEffect(Color.red, turns: turns));
			break;				
		default:
			break;
		}
		return effects;
	}
}