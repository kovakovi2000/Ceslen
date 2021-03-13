using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressingUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool Pressing = false;
    public bool Hover = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hover = false;
    }
}
