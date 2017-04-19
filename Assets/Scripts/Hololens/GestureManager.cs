using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : Singleton<GestureManager>
{
    /// <summary>
    /// To Select the Gameobject
    /// </summary>
    public GestureRecognizer SelectRecognizer
    {
        get;
        private set;
    }

    /// <summary>
    /// Currectly actived Recognizer
    /// </summary>
    public GestureRecognizer ActiveRecognizer
    {
        get;
        private set;
    }

    /// <summary>
    /// When sth is selected,this recongnizer will be active
    /// </summary>
    public GestureRecognizer ManipulationRecognizer
    {
        get;
        private set;
    }

    /// <summary>
    /// When sth is selected,this recongnizer will be active
    /// </summary>
    public GestureRecognizer NavigationRecognizer
    {
        get;
        private set;
    }

    public bool IsNavigation = false;
    public Vector3 NavigationRelativePosition
    {
        get;
        private set;
    }


    private void OnEnable()
    {
        SelectRecognizer = new GestureRecognizer();
        SelectRecognizer.SetRecognizableGestures(GestureSettings.Tap|GestureSettings.DoubleTap);

        SelectRecognizer.TappedEvent += TapRecognizer_TappedEvent;

        NavigationRecognizer = new GestureRecognizer();
        NavigationRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap | GestureSettings.NavigationRailsX);
        NavigationRecognizer.NavigationStartedEvent += NavigationRecognizer_Start;
        NavigationRecognizer.NavigationUpdatedEvent += NavigationRecognizer_Update;
        NavigationRecognizer.NavigationCompletedEvent += NavigationRecognizer_Completed;
        NavigationRecognizer.NavigationCanceledEvent += NavigationRecognizer_Canceled;
        NavigationRecognizer.TappedEvent += TapRecognizer_TappedEvent;

        //the default recognizer is to navigate
        SwitchRecognizer(SelectRecognizer);
    }


    private void OnDestroy()
    {
        SelectRecognizer.TappedEvent -= TapRecognizer_TappedEvent;

        NavigationRecognizer.NavigationStartedEvent -= NavigationRecognizer_Start;
        NavigationRecognizer.NavigationUpdatedEvent -= NavigationRecognizer_Update;
        NavigationRecognizer.NavigationCompletedEvent -= NavigationRecognizer_Completed;
        NavigationRecognizer.NavigationCanceledEvent -= NavigationRecognizer_Canceled;
        NavigationRecognizer.TappedEvent -= TapRecognizer_TappedEvent;
    }

    /// <summary>
    /// to switch recongnizer in cast there are not only one recognizer
    /// </summary>
    /// <param name="newRecognizer"> The recognizer to be started</param>
    public void SwitchRecognizer(GestureRecognizer newRecognizer)
    {
        if (newRecognizer == null)
        {
            return;
        }
        if (ActiveRecognizer != null)
        {
            if (ActiveRecognizer == newRecognizer)
            {
                return;
            }
            ActiveRecognizer.CancelGestures();
            ActiveRecognizer.StopCapturingGestures();
        }
        newRecognizer.StartCapturingGestures();
        ActiveRecognizer = newRecognizer;
    }

    private void TapRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (tapCount != 2)
        {
            return;
        }
        GameObject _focusedObject = InteractManager.Instance.FocusGameObject;
        if (_focusedObject != null)
        {
            _focusedObject.SendMessage("OnSelect");
        }
    }

    private void NavigationRecognizer_Start(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        Debug.Log("Start Navigation");
        IsNavigation = true;
        NavigationRelativePosition = RelativePosition;
    }
    private void NavigationRecognizer_Update(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        Debug.Log("Update Navigation");
        IsNavigation = true;
        NavigationRelativePosition = RelativePosition;
        Debug.Log("Relactive position is " + NavigationRelativePosition);
    }
    private void NavigationRecognizer_Completed(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        Debug.Log("Completed Navigation");
        IsNavigation = false;
    }
    private void NavigationRecognizer_Canceled(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        Debug.Log("Cancel Navigation");
        IsNavigation = false;
    }
}
