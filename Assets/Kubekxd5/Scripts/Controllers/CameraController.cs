using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerShip;

    [Header("Zoom Settings")] public float minZoom = 5f;

    public float maxZoom = 20f;
    public float zoomSpeed = 5f;

    [Header("Camera Offset Settings")] public float forwardOffset = 5f;

    public float followRange = 5f;

    private Vector3 _baseOffset;
    private Vector3 _cursorOffset;

    private void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnPlayerShipSpawned += OnPlayerShipSpawned;
    }

    private void LateUpdate()
    {
        if (playerShip != null)
        {
            HandleZoom();
            UpdateCursorOffset();
            UpdateCameraPositionAndRotation();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnPlayerShipSpawned -= OnPlayerShipSpawned;
    }

    private void OnPlayerShipSpawned()
    {
        FindPlayerShip();
    }

    private void HandleZoom()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            // Adjust the _baseOffset.y based on scroll input
            _baseOffset.y -= scrollInput * zoomSpeed;

            // Clamp the zoom level between minZoom and maxZoom
            _baseOffset.y = Mathf.Clamp(_baseOffset.y, minZoom, maxZoom);
        }
    }

    private void UpdateCursorOffset()
    {
        // Convert mouse position to viewport position (range from 0 to 1 on both axes)
        var mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // Center the viewport range around (0,0) instead of (0.5,0.5)
        var centeredViewportPos = new Vector3(mouseViewportPos.x - 0.5f, 0, mouseViewportPos.y - 0.5f);

        // Calculate cursor offset within the follow range, using local axes of the player ship
        _cursorOffset = Vector3.ClampMagnitude(centeredViewportPos * followRange, followRange);
        _cursorOffset = playerShip.transform.right * _cursorOffset.x + playerShip.transform.forward * _cursorOffset.z;
    }

    private void UpdateCameraPositionAndRotation()
    {
        // Calculate the base offset and forward offset based on the player's local space
        var forwardPosition = playerShip.transform.position + playerShip.transform.forward * forwardOffset;

        // Combine the base zoom offset, forward offset, and local cursor offset to determine final camera position
        var targetPosition = forwardPosition + new Vector3(_cursorOffset.x, _baseOffset.y, _cursorOffset.z);
        transform.position = targetPosition;

        // Rotate the camera to look at the player from the top, keeping rotation in sync with the player's rotation
        transform.rotation = Quaternion.Euler(90f, playerShip.transform.eulerAngles.y, 0f);
    }

    public void FindPlayerShip()
    {
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        if (playerShip != null)
            _baseOffset = transform.position - playerShip.transform.position;
        else
            Debug.LogWarning("No player ship found. Please assign a ship or ensure it has the 'PlayerShip' tag.");
    }
}