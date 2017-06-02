using UnityEngine;
using System;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static event EventHandler<InfoEventArgs<Vector3>> moveEvent;

	public float delta = 10.0f;
	public float speed = 3.0f;

	private float _tilemapSizeX;
	private float _tileMapSizeZ;

	private Vector3 _oldPosition = Vector3.zero;

	/// <summary>
	/// Init the specified tileMapSizeX and tileMapSizeY.
	/// </summary>
	/// <param name="tileMapSizeX">Tile map size x.</param>
	/// <param name="tileMapSizeY">Tile map size y.</param>
	/// <param name="tileResolution>Resolution of each tile.</param>"> 
	public void Init(float tileMapSizeX, float tileMapSizeZ, int tileResolution) {
		print ("CameraController.Init()");
		_tilemapSizeX = tileMapSizeX;
		_tileMapSizeZ = tileMapSizeZ;

		transform.rotation = Quaternion.Euler (90, 0, 0);
		Camera.main.orthographicSize = Screen.width / (((Screen.width / Screen.height) * 2) * 16);
	}
		
	// Update is called once per frame
	void Update () {

		if (!IsMoving) {
			Vector3 direction = Vector3.zero;

			Vector3 screenCoordinates = Input.mousePosition;
			Vector3 maxBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
			Vector3 minBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

			// Left
			if ((screenCoordinates.x <= 0.0f + delta || Input.GetKey(KeyCode.A)) && minBorderCoordinates.x >= 0.0f)
				direction -= Vector3.right;
			// Right
			if ((screenCoordinates.x >= Screen.width - delta || Input.GetKey(KeyCode.D)) && maxBorderCoordinates.x <= _tilemapSizeX)
				direction += Vector3.right;
			// Up
			if ((screenCoordinates.y >= Screen.height - delta || Input.GetKey(KeyCode.W)) && maxBorderCoordinates.z <= _tileMapSizeZ)
				direction += Vector3.forward;
			// Down
			if ((screenCoordinates.y <= 0.0f + delta || Input.GetKey(KeyCode.S)) && minBorderCoordinates.z >= 0.0f)
				direction -= Vector3.forward;

			transform.position += direction * Time.deltaTime * speed;

			// Send events when position changes
			// TODO: Move to InputController.cs
			if (transform.position != _oldPosition) {
				_oldPosition = transform.position;
				if (moveEvent != null)
					moveEvent (this, new InfoEventArgs<Vector3> (transform.position));
			}
		}
	}

	/// <summary>
	/// Snaps to position.
	/// </summary>
	/// <param name="newPosition">New position.</param>
	public void SnapToPosition(Vector3 newPosition) {
		IsMoving = true;
		newPosition.y = 10;
		transform.position = newPosition;
		IsMoving = false;
	}

	/// <summary>
	/// Smoothly moves the camera from the current position to another.
	/// </summary>
	/// <returns>The to position.</returns>
	/// <param name="endingPosition">Ending position.</param>
	public IEnumerator MoveToPosition(Vector3 endingPosition) {
		IsMoving = true;
		float elapsedTime = 0.0f;

		Vector3 startingPosition = transform.position;
		endingPosition.y = 10;

		float distance = Vector3.Distance (startingPosition, endingPosition);
		float timeToMove = distance * 0.0078125f;

		while (elapsedTime < timeToMove) {
			transform.position = Vector3.Lerp (startingPosition, endingPosition, (elapsedTime / timeToMove));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		IsMoving = false;
		yield return null;
	}

	/// <summary>
	/// Gets or sets a value indicating whether this instance is moving.
	/// </summary>
	/// <value><c>true</c> if this instance is moving; otherwise, <c>false</c>.</value>
	public bool IsMoving { get; set; }
}