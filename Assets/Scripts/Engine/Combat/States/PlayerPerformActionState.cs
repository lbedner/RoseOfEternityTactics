using UnityEngine;
using System.Collections;

public class PlayerPerformActionState : PerformActionState {

	public override void Enter() {
		print ("PlayerPerformActionState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
		controller.ClearActionTargets ();
		controller.Head2HeadPanel.SetActive (false);
		controller.ShowCursorAndTileSelector (false);
		Unit defender = controller.TileMap.GetTileMapData ().GetTileDataAt (controller.CurrentTileCoordinates).Unit;
		StartCoroutine (PerformAction (controller.HighlightedUnit, defender));		
	}
}