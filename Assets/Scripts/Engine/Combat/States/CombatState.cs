using UnityEngine;
using System.Collections;

public abstract class CombatState : State {

	protected CombatController controller;

	protected virtual void Awake() {
		controller = GetComponent<CombatController> ();
	}
}