using UnityEngine;
using System.Collections;

public class ExperienceManager {

	private const int BASE_AMOUNT = 10;
	private const int CONSUMABLE_BASE_AMOUNT = 5;

	private const int KILLING_BLOW_MODIFIER = 2;

	/// <summary>
	/// Awards the combat experience.
	/// </summary>
	/// <returns>The combat experience.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	public int AwardCombatExperience(Unit source, Unit target) {

		// Fall back if unit doesn't have experience attribute
		if (source.GetExperienceAttribute() == null)
			return 0;

		// Set base amount before modifications
		int xp = BASE_AMOUNT;

		// Factor in level difference
		int levelDifference = GetLevelDifference(source, target);
		if (levelDifference != 0)
			xp += levelDifference;
	
		// If target was killed, add modifier
		if ((int) target.GetHitPointsAttribute().CurrentValue <= 0)
			xp *= KILLING_BLOW_MODIFIER;

		// Increment XP
		source.GetExperienceAttribute().Increment(xp);

		return xp;
	}

	/// <summary>
	/// Awards the consumable experience.
	/// </summary>
	/// <returns>The consumable experience.</returns>
	public int AwardConsumableExperience(Unit source) {

		// Fall back if unit doesn't have experience attribute
		if (source.GetExperienceAttribute() == null)
			return 0;

		// Increment XP
		int xp = CONSUMABLE_BASE_AMOUNT;
		source.GetExperienceAttribute ().Increment (xp);

		return xp;
	}

	/// <summary>
	/// Gets the level difference between 2 units.
	/// </summary>
	/// <returns>The level difference.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	private int GetLevelDifference(Unit source, Unit target) {
		int sourceLevel = GetLevel (source);
		int targetLevel = GetLevel (target);

		if (sourceLevel != targetLevel)
			return targetLevel - sourceLevel;
		return 0;
	}

	/// <summary>
	/// Gets the level of the unit.
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="unit">Unit.</param>
	private int GetLevel(Unit unit) {
		return (int) unit.GetLevelAttribute().CurrentValue;
	}
}