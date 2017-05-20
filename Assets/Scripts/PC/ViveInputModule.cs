using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViveInputModule : BaseInputModule
{
	public bool GUIHit=false;

	private Camera eventCamera;

	private SteamVR_TrackedObject[] controllers;
	private SteamVR_Controller.Device[] controllerDevices;

	private PointerEventData[] PointEvents;

	private GameObject[] currentGaze;
	private GameObject[] currentPress;
	private GameObject[] currentDragging;

	protected override void Start()
	{
		base.Start ();
		eventCamera = new GameObject ("EventCamera").AddComponent<Camera> ();
		eventCamera.clearFlags = CameraClearFlags.Nothing;
		eventCamera.cullingMask = 0;

		SteamVR_ControllerManager ControllerManager=FindObjectOfType<SteamVR_ControllerManager>();
		controllers = new SteamVR_TrackedObject[2] {
			ControllerManager.left.GetComponent<SteamVR_TrackedObject> (),
			ControllerManager.right.GetComponent<SteamVR_TrackedObject> ()
		};
		controllerDevices = new SteamVR_Controller.Device[2];

		PointEvents = new PointerEventData[2];
		currentGaze = new GameObject[2];
		currentPress = new GameObject[2];
		currentDragging = new GameObject[2];


		Canvas[] canvases = FindObjectsOfType<Canvas> ();
		foreach (Canvas canvas in canvases) 
		{
			canvas.worldCamera = eventCamera;
		}
	}

	public override void Process()
	{
		Debug.Log ("Process");
		InitControllers ();
		GUIHit = false;

		SendUpdateEventToSelectedGameObject ();

		for (int index = 0; index != controllers.Length; ++index)
		{
			UpdateCameraPosition(index);
			GetPointerEventData (index);
			currentGaze [index] = PointEvents [index].pointerCurrentRaycast.gameObject;
			base.HandlePointerExitAndEnter (PointEvents [index], currentGaze [index]);
			if (controllers [index] != null)
			{
				if (isButtonDown (index))
				{
					ClearSelectGameobjcet ();
					PointEvents [index].pressPosition = PointEvents [index].position;
					PointEvents [index].pointerPressRaycast = PointEvents [index].pointerCurrentRaycast;
					PointEvents [index].pointerPress = null;
					if (currentGaze [index] != null)
					{
						currentPress [index] = currentGaze [index];
						GameObject newpressed = ExecuteEvents.ExecuteHierarchy (currentPress [index], PointEvents [index], ExecuteEvents.pointerDownHandler);
						if (newpressed == null)
						{
							newpressed = ExecuteEvents.ExecuteHierarchy (currentPress [index], PointEvents [index], ExecuteEvents.pointerClickHandler);	
						} 
						else
						{
							ExecuteEvents.Execute (newpressed, PointEvents [index], ExecuteEvents.pointerClickHandler);
						}

						if (newpressed != null)
						{
							PointEvents [index].pointerPress = newpressed;
							currentPress [index] = newpressed;
							if (ExecuteEvents.GetEventHandler<ISelectHandler> (newpressed))
							{
								base.eventSystem.SetSelectedGameObject (newpressed);
							}
						}
						ExecuteEvents.Execute (currentPress [index], PointEvents [index], ExecuteEvents.beginDragHandler);
						PointEvents [index].pointerDrag = currentPress [index];
						currentDragging [index] = currentPress [index];
					}
				}
				if (isButtonUp (index))
				{
					if (currentDragging [index])
					{
						ExecuteEvents.Execute (currentDragging[index], PointEvents [index], ExecuteEvents.endDragHandler);
						if (currentGaze [index])
						{
							ExecuteEvents.Execute (currentGaze [index], PointEvents [index], ExecuteEvents.dropHandler);
						}
						PointEvents [index].pointerDrag = null;
						currentDragging [index] = null;
					}
					if (currentPress [index])
					{
						ExecuteEvents.Execute (currentPress [index], PointEvents [index], ExecuteEvents.pointerUpHandler);
						PointEvents [index].pointerPress = null;
						PointEvents [index].rawPointerPress = null;
						currentPress [index] = null;
					}
				}
				if (currentDragging[index]!= null)
				{
					ExecuteEvents.Execute (currentDragging [index], PointEvents [index], ExecuteEvents.dragHandler);
				}
			}
		}


	}

	private bool isButtonDown(int index)
	{
		return controllerDevices [index] != null && controllerDevices [index].GetPressDown (SteamVR_Controller.ButtonMask.Trigger);
	}

	private bool isButtonUp(int index)
	{
		return controllerDevices [index] != null && controllerDevices [index].GetPressUp (SteamVR_Controller.ButtonMask.Trigger);
	}

	private void ClearSelectGameobjcet()
	{
		if (base.eventSystem.currentSelectedGameObject) 
		{
			base.eventSystem.SetSelectedGameObject (null);
		}
	}

	private void InitControllers()
	{
		for (int index = 0; index < controllers.Length; ++index)
		{
			if (controllers [index] != null) {
				controllerDevices [index] = SteamVR_Controller.Input ((int)controllers [index].index);
			} 
			else
			{
				controllerDevices [index] = null;
			}
		}
	}

	private bool SendUpdateEventToSelectedGameObject()
	{
		GameObject _selectedGameobject = eventSystem.currentSelectedGameObject;//this is auto set by unity
		if (_selectedGameobject == null) {
			Debug.Log ("There is No selected GameObject");
			return false;
		}
		else
		{
			Debug.Log ("Selected Gameobject is " + _selectedGameobject.name);
		}

		BaseEventData data = GetBaseEventData ();
		ExecuteEvents.Execute (_selectedGameobject, data, ExecuteEvents.updateSelectedHandler);

		return data.used;
	}

	private void UpdateCameraPosition(int controllerIndex)
	{
		eventCamera.transform.position = controllers [controllerIndex].transform.position;
		eventCamera.transform.forward = controllers [controllerIndex].transform.forward;
	}

	private void GetPointerEventData(int eventIndex)
	{
		if (PointEvents [eventIndex] == null) {
			PointEvents [eventIndex] = new PointerEventData (eventSystem);
		} 
		else
		{
			PointEvents [eventIndex].Reset ();
		}

		PointEvents [eventIndex].delta = Vector2.zero;
		PointEvents [eventIndex].position = new Vector2 (Screen.width / 2, Screen.height / 2);
		eventSystem.RaycastAll (PointEvents [eventIndex], m_RaycastResultCache);
		PointEvents [eventIndex].pointerCurrentRaycast = FindFirstRaycast (m_RaycastResultCache);
		if (PointEvents [eventIndex].pointerCurrentRaycast.gameObject != null) {
			Debug.Log ("Hit something " + PointEvents [eventIndex].pointerCurrentRaycast.gameObject.name);
			GUIHit = true;
		}
		else
		{
			Debug.Log ("No thing");
		}
		m_RaycastResultCache.Clear();
	}
}
