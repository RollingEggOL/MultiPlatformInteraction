using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerManager :Singleton<ViveControllerManager>
{

	public Transform CameraRig;

	private SteamVR_TrackedObject _leftController;
	private SteamVR_TrackedObject _rightController;

	private ComRef<SteamVR_Controller.Device> _leftDevices;
	private ComRef<SteamVR_Controller.Device> _rightDevices;

	public delegate void LeftTriggerPressEvent();
	public event LeftTriggerPressEvent _leftTriggerEvent;

	// Use this for initialization
	void Start () 
	{
		_leftController = CameraRig.Find ("Controller (left)").GetComponent<SteamVR_TrackedObject> ();
		_rightController = CameraRig.Find ("Controller (right)").GetComponent<SteamVR_TrackedObject> ();

		_leftDevices = new ComRef<SteamVR_Controller.Device> (() =>
			{
				return SteamVR_Controller.Input ((int)_leftController.index);
			});

		_rightDevices = new ComRef<SteamVR_Controller.Device> (() =>
			{
				return SteamVR_Controller.Input ((int)_rightController.index);
			});
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_leftDevices.Ref == null || _rightDevices.Ref == null) 
		{
			return;
		}
		if (_leftDevices.Ref.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) 
		{
			if (_leftTriggerEvent != null) 
			{
				_leftTriggerEvent ();
			}
			Debug.Log ("left trigger down");
		}

		if (_leftDevices.Ref.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) 
		{
			Debug.Log ("left manu press down");
			GameObject _focusGameobject = InteractManager.Instance.FocusGameObject;
			if (_focusGameobject != null)
			{
				_focusGameobject.SendMessage ("OnSelect");
			}
		}

		if (_rightDevices.Ref.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) 
		{
			Debug.Log ("right trigger down");
		}
	}
}
