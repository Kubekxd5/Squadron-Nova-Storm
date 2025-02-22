using UnityEngine;

public class AdvancedBuilding : MonoBehaviour
{
    [Header("Tracking Settings")] public bool canTrackPlayer;

    public float detectionRange = 20f;
    public float rotationSpeed = 2f;
    public bool rotateXAxis;
    public bool rotateYAxis = true;
    public bool rotateZAxis;

    [Header("Rotating Objects Settings")] public Transform[] rotatingObjects;

    public bool rotateOnDetection;

    private Transform _player;

    private void Start()
    {
        if (canTrackPlayer) FindPlayer();
    }

    private void Update()
    {
        if (canTrackPlayer && (_player == null || !IsPlayerInRange())) FindPlayer();

        if (canTrackPlayer && _player != null && IsPlayerInRange()) RotateTowardsPlayer();

        if (rotateOnDetection)
        {
            if (_player != null && IsPlayerInRange()) RotateIndefinitely();
        }
        else
        {
            RotateIndefinitely();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (canTrackPlayer)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        Gizmos.color = Color.yellow;
    }

    private void FindPlayer()
    {
        var playerObject = GameObject.FindWithTag("PlayerShip");
        if (playerObject != null)
            _player = playerObject.transform;
        else
            _player = null;
    }

    private bool IsPlayerInRange()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= detectionRange;
    }

    private void RotateTowardsPlayer()
    {
        if (_player == null) return;

        var directionToPlayer = _player.position - transform.position;
        var targetRotation = Quaternion.LookRotation(directionToPlayer);

        var targetEulerAngles = targetRotation.eulerAngles;
        var currentEulerAngles = transform.eulerAngles;

        if (!rotateXAxis) targetEulerAngles.x = currentEulerAngles.x;
        if (!rotateYAxis) targetEulerAngles.y = currentEulerAngles.y;
        if (!rotateZAxis) targetEulerAngles.z = currentEulerAngles.z;

        var constrainedRotation = Quaternion.Euler(targetEulerAngles);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            constrainedRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void RotateIndefinitely()
    {
        var rotation = Vector3.zero;

        if (rotateXAxis) rotation.x = rotationSpeed * Time.deltaTime;
        if (rotateYAxis) rotation.y = rotationSpeed * Time.deltaTime;
        if (rotateZAxis) rotation.z = rotationSpeed * Time.deltaTime;

        foreach (var objectToRotate in rotatingObjects) objectToRotate.transform.Rotate(rotation, Space.Self);
    }
}