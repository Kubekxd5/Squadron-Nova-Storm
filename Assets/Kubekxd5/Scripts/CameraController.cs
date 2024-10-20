using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 fixedPosition = player.transform.position + new Vector3(0, _offset.y, 0);
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        }
    }
}
