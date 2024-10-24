using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerShip;
    private Vector3 _offset;

    private void Start()
    {
        FindPlayerShip();
    }
    
    private void LateUpdate()
    {
        if (playerShip != null)
        {
            Vector3 fixedPosition = playerShip.transform.position + new Vector3(0, _offset.y, 0);
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(90f, playerShip.transform.eulerAngles.y, 0f);
        }
    }
    
    public void FindPlayerShip()
    {
        if (playerShip == null)
        {
            playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        }
        if (playerShip != null)
        {
            _offset = transform.position - playerShip.transform.position;
        }
        else
        {
            Debug.LogWarning("No player ship found. Please assign a ship or ensure it has the 'PlayerShip' tag.");
        }
    }
}
