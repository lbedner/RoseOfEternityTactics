using UnityEngine;
using System.Collections;

public class Combat {

	private Unit _attacker;
	private Unit _defender;

	/// <summary>
	/// Initializes a new instance of the <see cref="Combat"/> class.
	/// </summary>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	public Combat(Unit attacker, Unit defender) {
		_attacker = attacker;
		_defender = defender;
	}

	/// <summary>
	/// Begin the combat action(s).
	/// </summary>
	public void Begin() {
		int damage = GetDamage ();

		// Damage defender
		_defender.CurrentHitPoints -= damage;
		Debug.Log (string.Format ("{0}: {1} Damage -> {2}", _attacker, damage, _defender));

		// Show pop up text for damage
		PopupTextController.Initialize(_defender.GetCanvas());
		PopupTextController.CreatePopupText (damage.ToString (), _defender.transform.position);

		// If dead, then destroy
		if (_defender.CurrentHitPoints <= 0) {
			GameManager.Instance.GetTurnOrderController ().RemoveUnit (_defender);
			GameManager.Instance.GetTileMap ().GetTileMapData ().GetTileDataAt (_defender.Tile).Unit = null;
			GameObject.Destroy (_defender.gameObject, 1.0f);
		}
		else
			_defender.UpdateHealthbar ();
	}

	/// <summary>
	/// Gets the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	private int GetDamage() {
		int damage = _attacker.level * 2;
		return damage;
	}
}