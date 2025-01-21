using UnityEngine;
using UnityEngine.UI;

public class GarageCameraController : MonoBehaviour
{
    public GameObject[] garageCameras, cameraCurrentIcon;
    public int currentIndex;

    private void Start()
    {
        foreach (var cam in garageCameras) cam.SetActive(false);
        garageCameras[0].SetActive(true);
        currentIndex = 0;
        UpdateIcons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SwitchCamera(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchCamera(-1);
    }

    public void SetCamera(int index)
    {
        currentIndex = index;
        foreach (var cam in garageCameras) cam.SetActive(false);
        garageCameras[currentIndex].SetActive(true);
        UpdateIcons();
    }

    private void SwitchCamera(int direction)
    {
        garageCameras[currentIndex].SetActive(false);

        currentIndex += direction;

        if (currentIndex >= garageCameras.Length)
            currentIndex = 0;
        else if (currentIndex < 0) currentIndex = garageCameras.Length - 1;

        garageCameras[currentIndex].SetActive(true);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (var i = 0; i < cameraCurrentIcon.Length; i++)
            cameraCurrentIcon[i].GetComponent<Outline>().enabled = i == currentIndex;
    }
}