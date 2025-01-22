using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;

    [Header("Weapon Settings")]
    public List<WeaponController> equippedWeapons;

    [Header("Shooting Settings")]
    public float minShootInterval = 1f;
    public float maxShootInterval = 3f;
    public float aimPrecision = 0.1f; // 0 = is ignored

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool rotateXAxis;
    public bool rotateYAxis;
    public bool rotateZAxis;

    [Header("AI Settings")]
    public bool isAggressive = true;
    public bool isTrap;
    public float detectionRange = 15f; // 0 = infinite range
    public LayerMask playerLayer;

    [Header("Trap Settings")]
    public Collider trapTrigger; // Assignable trigger collider for traps

    private float _currentShootInterval;
    private float _shootTimer;

    private void Start()
    {
        player = GameObject.FindWithTag("PlayerShip")?.transform;
        _currentShootInterval = Random.Range(minShootInterval, maxShootInterval);

        if (isTrap && trapTrigger != null)
            trapTrigger.isTrigger = true;
    }

    private void Update()
    {
        if (!isTrap)
        {
            if (player == null)
            {
                SearchForPlayer();
                return;
            }

            if (IsPlayerInRange() && isAggressive)
            {
                RotateWeaponsTowardsPlayer();
                HandleShooting();
            }
            else if (!isAggressive || !isTrap)
            {
                RotateWeaponsIndefinitely();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange > 0 ? detectionRange : 50f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTrap || !other.CompareTag("PlayerShip")) return;
        Debug.Log("Player entered trap range. Starting to shoot.");
        player = other.transform;
        StartCoroutine(TrapShootingRoutine());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isTrap || !other.CompareTag("PlayerShip")) return;
        player = null;
    }

    private IEnumerator TrapShootingRoutine()
    {
        while (player != null)
        {
            TryShoot();
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    private void SearchForPlayer()
    {
        var detectedObjects = Physics.OverlapSphere(transform.position,
            detectionRange > 0 ? detectionRange : Mathf.Infinity, playerLayer);
        foreach (var obj in detectedObjects)
        {
            if (obj.CompareTag("PlayerShip"))
            {
                player = obj.transform;
                Debug.Log("Player detected and locked on.");
                break;
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;

        if (detectionRange <= 0) return true;
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    private void RotateWeaponsTowardsPlayer()
    {
        if (player == null || equippedWeapons == null) return;

        foreach (var weapon in equippedWeapons)
        {
            if (weapon == null) continue;

            var directionToPlayer = player.position - weapon.transform.position;
            var targetRotation = Quaternion.LookRotation(directionToPlayer);

            var targetEulerAngles = targetRotation.eulerAngles;
            var currentEulerAngles = weapon.transform.eulerAngles;

            if (!rotateXAxis) targetEulerAngles.x = currentEulerAngles.x;
            if (!rotateYAxis) targetEulerAngles.y = currentEulerAngles.y;
            if (!rotateZAxis) targetEulerAngles.z = currentEulerAngles.z;

            var constrainedRotation = Quaternion.Euler(targetEulerAngles);

            weapon.transform.rotation = Quaternion.Slerp(
                weapon.transform.rotation,
                constrainedRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void RotateWeaponsIndefinitely()
    {
        if (equippedWeapons == null) return;

        foreach (var weapon in equippedWeapons)
        {
            if (weapon == null) continue;

            var rotation = Vector3.zero;

            if (rotateXAxis) rotation.x = rotationSpeed * Time.deltaTime;
            if (rotateYAxis) rotation.y = rotationSpeed * Time.deltaTime;
            if (rotateZAxis) rotation.z = rotationSpeed * Time.deltaTime;

            weapon.transform.Rotate(rotation, Space.Self);
        }
    }

    private void HandleShooting()
    {
        _shootTimer += Time.deltaTime;

        if (player == null) return;

        var directionToPlayer = (player.position - transform.position).normalized;
        var aimOffset = Vector3.Angle(transform.forward, directionToPlayer);

        if (_shootTimer >= _currentShootInterval && (aimPrecision == 0 || aimOffset <= aimPrecision))
        {
            TryShoot();
            _shootTimer = 0f;
            _currentShootInterval = Random.Range(minShootInterval, maxShootInterval);
        }
    }

    private void TryShoot()
    {
        if (equippedWeapons == null || equippedWeapons.Count == 0) return;

        foreach (var weapon in equippedWeapons)
        {
            if (weapon != null)
                weapon.Shoot();
        }
    }
}
