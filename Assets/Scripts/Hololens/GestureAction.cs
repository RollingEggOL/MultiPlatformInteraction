using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAction : MonoBehaviour
{
    [Tooltip("How fast the gameobject will rotate by the value of navigation")]
    public float RotationSensitivity = 3f;

    public static bool IsRotating = false;

    private void Update()
    {
        PerformRotation();       
    }

    private void PerformRotation()
    {
        GameObject _selected = Interact.SelectedGameObject;

        //if we did not select anyGameobject then we will rotate the whole Brat
        //(all component in brat will enter this funciton and be rotated)
        //else we only rotate the select component
        //if we are draging slider,the gameobject should not be rotated
        if (_selected == null || _selected == InteractManager.Instance.FocusGameObject)
        {
            if (GestureManager.Instance.IsNavigation && !MultiSlider.IsDragging)
            {
                float rotationFactor = GestureManager.Instance.NavigationRelativePosition.x * RotationSensitivity;
                transform.Rotate(0, -1 * rotationFactor, 0);
                IsRotating = true;
            }
            else
            {
                IsRotating = false;
            }
        }
    }
}
