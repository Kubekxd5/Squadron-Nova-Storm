using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraButtonListener : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public GarageCameraController cameraController;
    private Outline outline;
    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        cameraController.SetCamera(index);
        outline.effectColor = Color.black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cameraController.currentIndex != index)
        {
            outline.enabled = true;
            outline.effectColor = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cameraController.currentIndex != index)
        {
            outline.effectColor = Color.black;
            outline.enabled = false;
        }
    }
}
