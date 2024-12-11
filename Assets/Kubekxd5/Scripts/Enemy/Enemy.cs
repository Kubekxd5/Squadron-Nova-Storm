using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum MovementType
    {
        Follow,
        Patrol,
        Idle
    }

    public MovementType movementType;

    [Header("Ogólne Ustawienia")] public Transform player;
    public float speed = 5f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;

    [Header("Ustawienia Patrolu")] public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    void Update()
    {
        // Sprawdź, czy transform gracza jest ustawiony, jeśli nie, spróbuj go znaleźć
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerShip");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log($"Znaleziono obiekt gracza: {playerObject.name}");
            }
            else
            {
                Debug.Log("Nie podano obiekt gracza!");
                return;
            }
        }

        switch (movementType)
        {
            case MovementType.Follow:
                FollowPlayer();
                break;
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Idle:
                Idle();
                break;
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > stoppingDistance)
            {
                transform.position += directionToPlayer * speed * Time.deltaTime;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 directionToPoint = (targetPoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float distanceToPoint = Vector3.Distance(transform.position, targetPoint.position);
        if (distanceToPoint > stoppingDistance)
        {
            transform.position += directionToPoint * speed * Time.deltaTime;
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void Idle()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}