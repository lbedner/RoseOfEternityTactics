using UnityEngine;
using System.Collections;

public class AbstractMapCameraController : MonoBehaviour
{
    public float delta = 10.0f;
    public float speed = 3.0f;

    protected float _tilemapSizeX;
    protected float _tileMapSizeZ;

    private Vector3 _oldPosition = Vector3.zero;

    /// <summary>
    /// Snaps to position.
    /// </summary>
    /// <param name="newPosition">New position.</param>
    public void SnapToPosition(Vector3 newPosition)
    {
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
    public IEnumerator MoveToPosition(Vector3 endingPosition)
    {
        IsMoving = true;
        float elapsedTime = 0.0f;

        Vector3 startingPosition = transform.position;
        endingPosition.y = 10;

        float distance = Vector3.Distance(startingPosition, endingPosition);
        float timeToMove = distance * 0.0078125f;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startingPosition, endingPosition, (elapsedTime / timeToMove));
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