using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndCombatState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("EndCombatState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.MusicController.StopAllMusic ();
		SceneManager.LoadScene ("ExampleScene");
	}
}