using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : Singleton<GestureManager>
{
    /// <summary>
    /// To rotation gameobject and select it
    /// </summary>
    public GestureRecognizer NavigationRecognizer
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

    private void OnEnable()
    {
        NavigationRecognizer = new GestureRecognizer();
        NavigationRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        NavigationRecognizer.TappedEvent += NavigationRecognizer_TappedEvent;

        //the default recognizer is to navigate
        SwitchRecognizer(NavigationRecognizer);
    }


    /// <summary>
    /// to switch recongnizer in cast there are not only one recognizer
    /// </summary>
    /// <param name="newRecognizer"> The recognizer to be started</param>
    void SwitchRecognizer(GestureRecognizer newRecognizer)
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

    void NavigationRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        GameObject _focusedObject = InteractManager.Instance.FocusGameObject;
        if (_focusedObject != null)
        {
            _focusedObject.SendMessage("OnSelect");
        }
    }
}
