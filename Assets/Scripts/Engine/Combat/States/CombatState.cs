using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatState : State {

	protected CombatController controller;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake() {
		controller = GetComponent<CombatController> ();
	}

	/// <summary>
	/// Handles the death.
	/// </summary>
	/// <param name="target">Target.</param>
	protected void HandleDeath(Unit target) {

		// Show death animation
		GameObject deathAnimation = Resources.Load<GameObject> ("Prefabs/Characters/Animations/Death/DeathParent");
		GameObject instance = GameObject.Instantiate (deathAnimation);
		Vector3 defenderPosition = target.transform.position;
		instance.transform.position = new Vector3 (defenderPosition.x, 0.1f, defenderPosition.z);
		GameObject.Destroy (instance, 1.5f);

		// Remove all highlighted tiles
		target.TileHighlighter.RemovePersistentHighlightedTiles ();

		var turnOrders = GameManager.Instance.GetTurnOrderController ().GetTurnOrderCollection ().GetTurnOrders ();
		foreach (var turnOrder in turnOrders)
			GameManager.Instance.GetTurnOrderController ().RemoveUnitAndAllImages (target, turnOrder);
		GameManager.Instance.GetTileMap ().GetTileMapData ().GetTileDataAt (target.Tile).Unit = null;
		GameManager.Instance.GetTileMap ().GetEnemies ().Remove (target);
		GameObject.Destroy (target.gameObject);
	}

	/// <summary>
	/// Applies the VFX to targets.
	/// </summary>
	/// <returns>The VFX to targets.</returns>
	/// <param name="vfxPath">Vfx path.</param>
	/// <param name="targets">Targets.</param>
	protected List<GameObject> ApplyVFXToTargets(string vfxPath, List<Unit> targets) {
		List<GameObject> vfxGameObjects = new List<GameObject> ();
		if (vfxPath != null && vfxPath != "") {
			GameObject VFXPrefab = Resources.Load<GameObject> (vfxPath);
			foreach (var target in targets) {
				GameObject VFX = Instantiate (VFXPrefab);
				Vector3 unitPositionWorld = TileMapUtil.TileMapToWorldCentered(target.Tile, controller.TileMap.TileSize);
				unitPositionWorld.y = 1; // TODO: Start using rendering layers to make appear in front of other objects
				VFX.transform.position = unitPositionWorld;
				vfxGameObjects.Add (VFX);
			}
		}
		return vfxGameObjects;
	}

	/// <summary>
	/// Destroys the VFX's.
	/// </summary>
	/// <param name="vfxGameObjects">Vfx game objects.</param>
	protected void DestroyVFX(List<GameObject> vfxGameObjects) {
		foreach (var vfx in vfxGameObjects)
			Destroy(vfx);		
	}
}