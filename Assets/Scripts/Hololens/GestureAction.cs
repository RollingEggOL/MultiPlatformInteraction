using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAction : MonoBehaviour
{
    [Tooltip("How fast the gameobject will rotate by the value of navigation")]
    public float RotationSensitivity = 3f;

    private void Update()
    {
        PerformRotation();       
    }

    private void PerformRotation()
    {
        GameObject _selected = Interact._selectedGameObject;

        //if we did not select anyGameobject then we will rotate the whole Brat
        //(all component in brat will enter this funciton and rotated)
        //else we only rotate the select component
        if (GestureManager.Instance.IsNavigation && 
            (_selected == null || (_selected != null && _selected == gameObject)))
        {
            float rotationFactor = GestureManager.Instance.NavigationRelativePosition.x * RotationSensitivity;
            transform.Rotate(0, -1 * rotationFactor, 0);
        }
    }
}
