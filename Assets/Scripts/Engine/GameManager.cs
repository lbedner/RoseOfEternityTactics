using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public TileMap tilemap;

	public enum gameState {
		INITIALIZATION,
		COMBAT
	}

	// Use this for initialization
	void Start () {
		tilemap.Initialize ();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
