using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// To Keeps the track of the gameobject which is currently in focus
/// </summary>
public class InteractManager : Singleton<InteractManager>
{
    public GameObject FocusGameObject
    {
        get;
        private set;
    }

    private GameObject OldFocusGameObject=null;

    private void Start()
    {
        FocusGameObject = null;
    }
    private void Update()
    {
        OldFocusGameObject = FocusGameObject;
        GazeManager _gazeManager=GazeManager.Instance;
        if (_gazeManager == null)
        {
            return;
        }
        FocusGameObject = null;
        if (_gazeManager.Hit)
        {
            RaycastHit _hitInfo = _gazeManager.HitInfo;
            if (_hitInfo.collider != null)
            {
                FocusGameObject = _hitInfo.collider.gameObject;
            }
        }

        if (FocusGameObject != OldFocusGameObject)
        {
            SendFocusState(FocusGameObject, true);
            SendFocusState(OldFocusGameObject, false);
        }
    }

    /// <summary>
    /// To send the gaze enter or exit message to gameobject
    /// </summary>
    /// <param name="GameobjectToBeHandled">
    /// The gameobject which the message is for</param>
    /// <param name="Focus">
    /// whether the gameobject is focused or not,if focused,then send the enter message</param>
    private void SendFocusState(GameObject GameobjectToBeHandled,bool Focus)
    {
        if (GameobjectToBeHandled != null)
        {
            if (GameobjectToBeHandled.GetComponent<Interact>()!= null)
            {
                if (Focus)
                {
                    GameobjectToBeHandled.SendMessage("GazeEnter");
                }
                else
                {
                    GameobjectToBeHandled.SendMessage("GazeExit");
                }
            }
        }
    }
}
