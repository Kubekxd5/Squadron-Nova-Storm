using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    public Transform centerPoint;
    public int rotateSpeed;

    private void Start()
    {
        rotateSpeed = 30;
    }

    private void Update()
    {
        transform.RotateAround(centerPoint.position, Vector3.up, rotateSpeed * Time.deltaTime);
        gameObject.transform.LookAt(centerPoint);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeRotationSpeed(KeyCode.UpArrow);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeRotationSpeed(KeyCode.DownArrow);
    }

    private void ChangeRotationSpeed(KeyCode keycode)
    {
        if (keycode == KeyCode.DownArrow)
        {
            rotateSpeed -= 5;
            if (rotateSpeed < 10) rotateSpeed = 10;
        }

        if (keycode == KeyCode.UpArrow)
        {
            rotateSpeed += 5;
            if (rotateSpeed > 100) rotateSpeed = 100;
        }
    }
}