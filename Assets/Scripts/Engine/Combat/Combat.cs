using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat {

	private Unit _attacker;
	private List<Unit> _defenders = new List<Unit>();

	private int _awardedExperience;

	/// <summary>
	/// Initializes a new instance of the <see cref="Combat"/> class.
	/// </summary>
	/// <param name="attacker">Attacker.</param>
	public Combat(Unit attacker) {
		_attacker = attacker;
		_defenders = attacker.Action.Targets;
	}

	/// <summary>
	/// Begin the combat action(s).
	/// </summary>
	public void Begin() {

		ExperienceManager manager = new ExperienceManager ();
		_awardedExperience = 0;

		// iterate over all defenders
		foreach (var defender in _defenders) {

			// Give out XP
			_awardedExperience += manager.AwardCombatExperience (_attacker, defender);
		}			
	}

	/// <summary>
	/// Gets the awarded experience.
	/// </summary>
	/// <returns>The awarded experience.</returns>
	public int GetAwardedExperience() {
		return _awardedExperience;
	}

	/// <summary>
	/// Gets the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	private int GetDamage() {
		Calculator calculator = new Calculator (_attacker);
		return calculator.Action.DamageToTargets [0];
	}
}