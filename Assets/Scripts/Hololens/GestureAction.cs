using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureAction : MonoBehaviour
{
    [Tooltip("How fast the gameobject will rotate by the value of navigation")]
    public float RotationSensitivity = 3f;

    [Range(0,1)]
    [Tooltip("How fast the gameobject will move by the value of manipulation")]
    public float ManipulationSensitivity = 0.05f;

    //when handling the gui or we detected that we are navigating in gestureManager,this flag should be true
    public static bool IsNavigating = false;

    private void Update()
    {
        PerformRotation();       
    }

    //TODO:Optimize this function,think wheter can remove it from the update function
    private void PerformRotation()
   { 
        GameObject _selected = Interact.SelectedGameObject;
        if (GestureManager.Instance == null)
        {
            return;
        }

        //if we did not select any Gameobject then we will rotate the whole Brat
        //(all component in brat will enter this funciton and be rotated)
        //else we only rotate the select component
        //if we are draging slider,the gameobject should not be rotated
        if (_selected == null || _selected == InteractManager.Instance.FocusGameObject)
        {
            if (GestureManager.Instance.IsNavigation && !MultiSlider.IsDragging)
            {
                float rotationFactor = GestureManager.Instance.NavigationRelativePosition.x * RotationSensitivity;
                transform.Rotate(0, -1 * rotationFactor, 0);
                IsNavigating = true;
            }
            else
            {
                IsNavigating = false;
            }
        }
    }


    private Vector3 manipulationPreviousPosition = Vector3.zero;

    private void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
    }

    private void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.IsManipulation)
        {
            Vector3 deltaVector = position - manipulationPreviousPosition;
            transform.position += deltaVector*ManipulationSensitivity;
        }
    }
}
