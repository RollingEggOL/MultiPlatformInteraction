using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : Singleton<GestureManager>
{
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
    public bool IsManipulation = false;
    public Vector3 NavigationRelativePosition
    {
        get;
        private set;
    }


    private void OnEnable()
    {
        NavigationRecognizer = new GestureRecognizer();
        NavigationRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap | GestureSettings.NavigationRailsX);
        NavigationRecognizer.NavigationStartedEvent += NavigationRecognizer_Start;
        NavigationRecognizer.NavigationUpdatedEvent += NavigationRecognizer_Update;
        NavigationRecognizer.NavigationCompletedEvent += NavigationRecognizer_Completed;
        NavigationRecognizer.NavigationCanceledEvent += NavigationRecognizer_Canceled;
        NavigationRecognizer.TappedEvent += Recognizer_TappedEvent;

        ManipulationRecognizer = new GestureRecognizer();
        ManipulationRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap | GestureSettings.ManipulationTranslate);
        ManipulationRecognizer.ManipulationStartedEvent += ManipulationRecognzer_Start;
        ManipulationRecognizer.ManipulationUpdatedEvent += ManipulationRecognzer_Update;
        ManipulationRecognizer.ManipulationCompletedEvent += ManipulationRecognzer_Completed;
        ManipulationRecognizer.ManipulationCanceledEvent += ManipulationRecognzer_Canceled;
        ManipulationRecognizer.TappedEvent += Recognizer_TappedEvent;


        //the default recognizer is to navigate
        SwitchRecognizer(NavigationRecognizer);
    }


    private void OnDisable()
    {
        NavigationRecognizer.NavigationStartedEvent -= NavigationRecognizer_Start;
        NavigationRecognizer.NavigationUpdatedEvent -= NavigationRecognizer_Update;
        NavigationRecognizer.NavigationCompletedEvent -= NavigationRecognizer_Completed;
        NavigationRecognizer.NavigationCanceledEvent -= NavigationRecognizer_Canceled;
        NavigationRecognizer.TappedEvent -= Recognizer_TappedEvent;

        ManipulationRecognizer.ManipulationStartedEvent -= ManipulationRecognzer_Start;
        ManipulationRecognizer.ManipulationUpdatedEvent -= ManipulationRecognzer_Update;
        ManipulationRecognizer.ManipulationCompletedEvent -= ManipulationRecognzer_Completed;
        ManipulationRecognizer.ManipulationCanceledEvent -= ManipulationRecognzer_Canceled;
        ManipulationRecognizer.TappedEvent -= Recognizer_TappedEvent;

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

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
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
        IsNavigation = true;
        NavigationRelativePosition = RelativePosition;
    }
    private void NavigationRecognizer_Update(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsNavigation = true;
        NavigationRelativePosition = RelativePosition;
    }
    private void NavigationRecognizer_Completed(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsNavigation = false;
    }
    private void NavigationRecognizer_Canceled(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsNavigation = false;
    }

    private void ManipulationRecognzer_Start(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsManipulation = true;
        Interact.SelectedGameObject.SendMessage("PerformManipulationStart", RelativePosition);
    }

    private void ManipulationRecognzer_Update(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsManipulation = true;
        Interact.SelectedGameObject.SendMessage("PerformManipulationUpdate", RelativePosition);
    }

    private void ManipulationRecognzer_Completed(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsManipulation = false;
    }

    private void ManipulationRecognzer_Canceled(InteractionSourceKind source, Vector3 RelativePosition, Ray headRay)
    {
        IsManipulation = false;
    }

}
