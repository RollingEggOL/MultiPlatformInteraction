using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// To track whether hand is detected or not
/// </summary>
public class HandsManager : Singleton<HandsManager>
{
    public bool HandDetected
    {
        get;
        private set;
    }

    private void Awake()
    {
        InteractionManager.SourceDetected += Source_Detected;
        InteractionManager.SourceLost += Source_Lost;
    }

    private void OnDestroy()
    {
        InteractionManager.SourceDetected -= Source_Detected;
        InteractionManager.SourceLost -= Source_Lost;
    }

    private void Source_Detected(InteractionSourceState hand)
    {
        HandDetected = true;
    }

    private void Source_Lost(InteractionSourceState hand)
    {
        HandDetected = false;
    }
}
