using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calculator {

	public Action Action { get; private set; }

	private Unit _source;
	private List<Unit> _targets;
	private List<int> _damageToTargets;

	/// <summary>
	/// Initializes a new instance of the <see cref="Calculator"/> class.
	/// </summary>
	/// <param name="action">Action.</param>
	public Calculator(Unit source) {
		_source = source;
		Action = _source.Action;
		_targets = Action.Targets;
		_damageToTargets = Action.DamageToTargets;

		CalculateDamage ();
	}


	/// <summary>
	/// Calculates the damage.
	/// </summary>
	private void CalculateDamage() {

		switch (Action.Ability.Id) {

		case AbilityConstants.ATTACK:
			CalculateAttackDamage ();
			break;

		case AbilityConstants.FLAME:
			CalculateFlameDamage ();
			break;

		case AbilityConstants.FREEZE:
			CalculateFreezeDamage ();
			break;

		case AbilityConstants.LIGHTNING:
			CalculateLightningDamage ();
			break;
		}
	}

	/// <summary>
	/// Calculates the attack damage.
	/// </summary>
	private void CalculateAttackDamage() {
		Item weapon = _source.GetItemInSlot (InventorySlots.SlotType.RIGHT_HAND);
		int damageAttribute = (int)weapon.GetAttribute (AttributeEnums.AttributeType.DAMAGE).CurrentValue;
		int damage = (int)_source.GetLevelAttribute ().CurrentValue * damageAttribute;
		_damageToTargets.Add (damage);
		Action.DamageToTargets = _damageToTargets;
	}

	/// <summary>
	/// Gets the calculated magic and level based damage.
	/// </summary>
	/// <param name="levelDivisor">Level divisor.</param>
	private void GetCalculatedMagicAndLevelBasedDamage(int levelDivisor) {
		foreach (var target in _targets) {
			float magic = _source.GetAttribute (AttributeEnums.AttributeType.MAGIC).CurrentValue;
			float level = _source.GetLevelAttribute ().CurrentValue;
			var damage = (magic * (level / levelDivisor));

			// Distribute the damage of the spell if multiple enemies
			if (_targets.Count > 1)
				damage /= (_targets.Count * 0.90f);
			
			_damageToTargets.Add(Mathf.CeilToInt(damage));
		}
		Action.DamageToTargets = _damageToTargets;
	}

	/// <summary>
	/// Calculates the flame damage.
	/// </summary>
	private void CalculateFlameDamage() {
		GetCalculatedMagicAndLevelBasedDamage (2);
	}

	/// <summary>
	/// Calculates the freeze damage.
	/// </summary>
	private void CalculateFreezeDamage() {
		GetCalculatedMagicAndLevelBasedDamage (4);
	}

	/// <summary>
	/// Calculates the lightning damage.
	/// </summary>
	private void CalculateLightningDamage() {
		GetCalculatedMagicAndLevelBasedDamage (3);
	}
}