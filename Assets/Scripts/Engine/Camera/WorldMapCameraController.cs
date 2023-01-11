using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapCameraController : AbstractMapCameraController 
{

	private InputAction _moveAction;

	private void Awake() {
		_moveAction = GetComponent<PlayerInput>().actions["Move"];
	}

    private void Start()
    {
        Init(35.0f * 16, 35.0f * 16, 16);
    }

    /// <summary>
    /// Init the specified tileMapSizeX and tileMapSizeY.
    /// </summary>
    /// <param name="tileMapSizeX">Tile map size x.</param>
    /// <param name="tileMapSizeZ">Tile map size y.</param>
    /// <param name="tileResolution">Resolution of each tile.</param>"> 
    public void Init(float tileMapSizeX, float tileMapSizeZ, int tileResolution)
    {
        print("WorldMapCameraController.Init()");
        _tilemapSizeX = tileMapSizeX;
        _tileMapSizeZ = tileMapSizeZ;

        //transform.rotation = Quaternion.Euler(90, 0, 0);
        //Camera.main.orthographicSize = Screen.width / (((Screen.width / Screen.height) * 2) * 16);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMoving)
        {

            Vector3 screenCoordinates = Input.mousePosition;
            Vector3 maxBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            Vector3 minBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

            Vector3 direction = _moveAction.ReadValue<Vector2>();
            transform.position += direction * Time.deltaTime * speed;
        }
    }
}
