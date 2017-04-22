using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MultiSlider : Slider,IPointerUpHandler,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    /// <summary>
    /// Whether the drag of slider is ready
    /// Set true after we select the slider
    /// Set false after we cancel the click
    /// </summary>
    public static bool IsDragging=false;
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        IsDragging = false;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        IsDragging = true;
    }
}
