using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyTower : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;

    [Header("Weapon Settings")]
    public GameObject weapon;
    public ParticleSystem[] gunfireVfx;
    public AudioSource gunfireSfx;

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

    private float _shootTimer;
    private float _currentShootInterval;

    private void Start()
    {
        player = GameObject.FindWithTag("PlayerShip")?.transform;
        _currentShootInterval = Random.Range(minShootInterval, maxShootInterval);

        if (isTrap && trapTrigger != null)
        {
            trapTrigger.isTrigger = true;
        }
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
                RotateTowardsPlayer();
                HandleShooting();
            }
            else if (!isAggressive || !isTrap)
            {
                RotateIndefinitely();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTrap && !other.CompareTag("PlayerShip")) return;
        Debug.Log("Player entered trap range. Starting to shoot.");
        player = other.transform;
        StartCoroutine(TrapShootingRoutine());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isTrap && !other.CompareTag("PlayerShip")) return;
        player = null;
    }

    private IEnumerator TrapShootingRoutine()
    {
        while (player != null)
        {
            Shoot();
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    private void SearchForPlayer()
    {
        Collider[] detectedObjects = Physics.OverlapSphere(transform.position, detectionRange > 0 ? detectionRange : Mathf.Infinity, playerLayer);
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

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - weapon.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        Vector3 currentEulerAngles = weapon.transform.eulerAngles;

        if (!rotateXAxis) targetEulerAngles.x = currentEulerAngles.x;
        if (!rotateYAxis) targetEulerAngles.y = currentEulerAngles.y;
        if (!rotateZAxis) targetEulerAngles.z = currentEulerAngles.z;

        Quaternion constrainedRotation = Quaternion.Euler(targetEulerAngles);

        weapon.transform.rotation = Quaternion.Slerp(
            weapon.transform.rotation,
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

        weapon.transform.Rotate(rotation, Space.Self);
    }

    private void HandleShooting()
    {
        _shootTimer += Time.deltaTime;

        Vector3 directionToPlayer = (player.position - weapon.transform.position).normalized;
        float aimOffset = Vector3.Angle(weapon.transform.forward, directionToPlayer);

        if (_shootTimer >= _currentShootInterval && (aimPrecision == 0 || aimOffset <= aimPrecision))
        {
            Shoot();
            _shootTimer = 0f;
            _currentShootInterval = Random.Range(minShootInterval, maxShootInterval);
        }
    }

    private void Shoot()
    {
        gunfireSfx.Play();
        foreach (var gun in gunfireVfx)
        {
            gun.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange > 0 ? detectionRange : 50f);
    }
}
