using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public enum MovementType
    {
        Chase,
        Follow,
        Patrol,
        Idle
    }

    public MovementType movementType;

    [Header("General Settings")]
    public Transform targetObject; // Object to follow (non-player)
    public Transform playerObject; // Reference to the player's transform
    public float speed = 5f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;

    [Header("Obstacle Avoidance Settings")]
    public float avoidanceDistance = 3f; // Distance to detect obstacles
    public float avoidanceStrength = 2f; // How strongly the enemy avoids obstacles
    public LayerMask obstacleLayer; // Layer for obstacles to avoid

    [Header("Weapon Settings")]
    public List<WeaponController> equippedWeapons;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public int patrolPointsCount = 5;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Idle Settings")]
    public float idleRotationInterval = 3f;
    private float nextIdleRotationTime;

    private bool isPlayerDetected;

    void Start()
    {
        GeneratePatrolPoints();
        FindPlayerObject();
    }

    void Update()
    {
        DetectPlayer();

        switch (movementType)
        {
            case MovementType.Chase:
                ChasePlayer();
                break;
            case MovementType.Follow:
                FollowTargetAndRotateToPlayer();
                break;
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Idle:
                Idle();
                break;
        }

        TryShoot();
    }

    private void FindPlayerObject()
    {
        if (playerObject == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("PlayerShip");
            if (player != null)
            {
                playerObject = player.transform;
                Debug.Log($"Player found: {player.name}");
            }
        }
    }

    private void DetectPlayer()
    {
        if (playerObject == null) FindPlayerObject();

        if (playerObject != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.position);
            isPlayerDetected = distanceToPlayer <= detectionRange;

            if (isPlayerDetected)
            {
                movementType = MovementType.Chase;
            }
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    private void ChasePlayer()
    {
        if (playerObject == null)
        {
            movementType = MovementType.Idle;
            return;
        }

        Vector3 directionToPlayer = (playerObject.position - transform.position).normalized;

        // Rotate towards the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move toward the player unless within stopping distance
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceToPlayer > stoppingDistance)
        {
            MoveWithAvoidance(directionToPlayer);
        }
    }

    private void FollowTargetAndRotateToPlayer()
    {
        if (targetObject == null || playerObject == null)
        {
            movementType = MovementType.Idle;
            return;
        }

        Vector3 directionToTarget = (targetObject.position - transform.position).normalized;

        // Follow the target object
        float distanceToTarget = Vector3.Distance(transform.position, targetObject.position);
        if (distanceToTarget > stoppingDistance)
        {
            MoveWithAvoidance(directionToTarget);
        }

        // Rotate to face the player
        Vector3 directionToPlayer = (playerObject.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Vector3 targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 directionToPoint = (targetPoint - transform.position).normalized;

        float distanceToPoint = Vector3.Distance(transform.position, targetPoint);
        if (distanceToPoint > stoppingDistance)
        {
            MoveWithAvoidance(directionToPoint);
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private float currentAngle;
    private float targetAngle;
    
    private void Idle()
    {
        if (Time.time >= nextIdleRotationTime)
        {
            targetAngle = Random.Range(0, 360);
            nextIdleRotationTime = Time.time + idleRotationInterval;
        }

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, currentAngle, 0);
    }

    private void TryShoot()
    {
        if (equippedWeapons != null && equippedWeapons.Count > 0 && isPlayerDetected)
        {
            foreach (var weapon in equippedWeapons)
            {
                if (weapon != null)
                {
                    weapon.Shoot();
                }
            }
        }
    }

    private void GeneratePatrolPoints()
    {
        if (patrolPointsCount <= 0 || patrolRadius <= 0)
        {
            Debug.LogWarning("Patrol points count or radius is invalid. Skipping patrol point generation.");
            return;
        }

        patrolPoints = new Vector3[patrolPointsCount];
        Vector3 center = transform.position;

        for (int i = 0; i < patrolPointsCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(0f, patrolRadius);
            float x = center.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = center.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);
            patrolPoints[i] = new Vector3(x, center.y, z);
        }
    }

    private void MoveWithAvoidance(Vector3 directionToTarget)
    {
        Vector3 avoidanceVector = Vector3.zero;
        RaycastHit hit;

        // Cast a sphere in the forward direction to detect obstacles
        if (Physics.SphereCast(transform.position, 3f, transform.forward, out hit, avoidanceDistance, obstacleLayer))
        {
            // Calculate avoidance direction
            avoidanceVector = Vector3.Cross(hit.normal, Vector3.up) * avoidanceStrength;
        }

        // Combine target movement and avoidance
        Vector3 movementDirection = directionToTarget + avoidanceVector;
        movementDirection = movementDirection.normalized;

        transform.position += movementDirection * speed * Time.deltaTime;
    }
}
