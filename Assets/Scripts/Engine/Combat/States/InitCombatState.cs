using UnityEngine;
using System.Collections;

public class InitCombatState : CombatState {

	private const float FADE_IN_DURATION = 2.0f;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		base.Enter ();
		StartCoroutine(Init ());
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private IEnumerator Init() {
		controller.ShowCursorAndTileSelector (false);
		ScreenFader screenFader = controller.ScreenFader;
		StartCoroutine(screenFader.FadeScreen (controller.FadeOutUIImage, ScreenFader.FadeType.FADE_IN, FADE_IN_DURATION));
		yield return new WaitForSeconds (FADE_IN_DURATION);
		controller.ChangeState<DisplayMissionObjectivesState> ();
	}
}