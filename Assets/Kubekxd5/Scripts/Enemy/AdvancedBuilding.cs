using UnityEngine;

public class AdvancedBuilding : MonoBehaviour
{
    [Header("Tracking Settings")]
    public bool canTrackPlayer = false;
    public float detectionRange = 20f;
    public float rotationSpeed = 2f;
    public bool rotateXAxis = false;
    public bool rotateYAxis = true;
    public bool rotateZAxis = false;

    [Header("Rotating Objects Settings")]
    public Transform[] rotatingObjects;
    public bool rotateOnDetection;

    private Transform _player;

    private void Start()
    {
        if (canTrackPlayer)
        {
            FindPlayer();
        }
    }

    private void Update()
    {
        if (canTrackPlayer && (_player == null || !IsPlayerInRange()))
        {
            FindPlayer();
        }

        if (canTrackPlayer && _player != null && IsPlayerInRange())
        {
            RotateTowardsPlayer();
        }

        if (rotateOnDetection)
        {
            if (_player != null && IsPlayerInRange())
            {
                RotateIndefinitely();
            }
        }
        else
        {
            RotateIndefinitely();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("PlayerShip");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            _player = null;
        }
    }

    private bool IsPlayerInRange()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= detectionRange;
    }

    private void RotateTowardsPlayer()
    {
        if (_player == null) return;

        Vector3 directionToPlayer = _player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        Vector3 currentEulerAngles = transform.eulerAngles;

        if (!rotateXAxis) targetEulerAngles.x = currentEulerAngles.x;
        if (!rotateYAxis) targetEulerAngles.y = currentEulerAngles.y;
        if (!rotateZAxis) targetEulerAngles.z = currentEulerAngles.z;

        Quaternion constrainedRotation = Quaternion.Euler(targetEulerAngles);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            constrainedRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void RotateIndefinitely()
    {
        Vector3 rotation = Vector3.zero;

        if (rotateXAxis) rotation.x = rotationSpeed * Time.deltaTime;
        if (rotateYAxis) rotation.y = rotationSpeed * Time.deltaTime;
        if (rotateZAxis) rotation.z = rotationSpeed * Time.deltaTime;

        foreach (var objectToRotate in rotatingObjects)
        {
            objectToRotate.transform.Rotate(rotation, Space.Self);
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
}
