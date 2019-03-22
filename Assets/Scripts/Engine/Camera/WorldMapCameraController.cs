using UnityEngine;
using System.Collections;

//public class WorldMapCameraController : MonoBehaviour
public class WorldMapCameraController : AbstractMapCameraController 
{

    private void Start()
    {
        //_tilemapSizeX = 35.0f * 16;
        //_tileMapSizeZ = 35.0f * 16;
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
            Vector3 direction = Vector3.zero;

            Vector3 screenCoordinates = Input.mousePosition;
            Vector3 maxBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            Vector3 minBorderCoordinates = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

            // Left
            if (Input.GetKey(KeyCode.A))
                direction -= Vector3.right;
            // Right
            if (Input.GetKey(KeyCode.D))
                direction += Vector3.right;
            // Up
            if (Input.GetKey(KeyCode.W))
                direction += Vector3.up;
            // Down
            if (Input.GetKey(KeyCode.S))
                direction -= Vector3.up;

            transform.position += direction * Time.deltaTime * speed;
        }
    }
}
