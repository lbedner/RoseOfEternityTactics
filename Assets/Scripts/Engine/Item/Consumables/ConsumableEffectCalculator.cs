using System;
using System.Collections.Generic;

public class ConsumableEffectCalculator
{
	/// <summary>
	/// Gets the consumable effects.
	/// </summary>
	/// <returns>The consumable effects.</returns>
	/// <param name="unit">Unit.</param>
	/// <param name="item">Item.</param>
	public static List<Effect.EffectDelegate> GetConsumableEffects(Unit unit, Item item) {
		List<Effect.EffectDelegate> effects = new List<Effect.EffectDelegate> ();
		switch (item.Id) {
			case ConsumableConstants.TONIC_OF_ROBUSTNESS_1:
			effects.Add(delegate { Effect.EffectHeal(unit, 10); });
			effects.Add(delegate { Effect.EffectSpeed(unit, 10, 3); });
				break;
		case ConsumableConstants.FLEETNESS_TONIC:
				effects.Add(delegate { Effect.EffectSpeed(unit, 5, 2); });
				break;				
			default:
				break;
		}
		return effects;
	}
}