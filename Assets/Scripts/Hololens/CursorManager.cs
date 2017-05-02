using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manager the Cursor State
/// </summary>
public class CursorManager : MonoBehaviour {

    [Tooltip("The cursor when it hits some gameobjects")]
    public GameObject CursorOn;

    [Tooltip("The cursor when it doesn't hit any gameobjects ")]
    public GameObject CursorOff;

    [Tooltip("The cursor when hololens can see user's hand")]
    public GameObject prefab_HandDetectedCursor;
    private GameObject handDetectedCursor;

    [Tooltip("The cursor when scrolling sth(navigating)")]
    public GameObject prefab_ScrollDetectedCursor;
    private GameObject scrollDetectedCursor;

    [Tooltip("The cursor when pathing the gameobject")]
    public GameObject prefab_PathingDetectedCursor;
    private GameObject pathDetectedCursor;

    [Tooltip("The hand or scroll cursor's parent in case to make them face the user")]
    public GameObject Billboard;

	void Start ()
    {
        if (CursorOn == null || CursorOff == null || prefab_HandDetectedCursor==null || prefab_PathingDetectedCursor==null)
        {
            Debug.LogError("THE CURSOR IS NOT SET");
            return;
        }

        handDetectedCursor = InstantiatePrefab(prefab_HandDetectedCursor);
        scrollDetectedCursor = InstantiatePrefab(prefab_ScrollDetectedCursor);
        pathDetectedCursor = InstantiatePrefab(prefab_PathingDetectedCursor);

        //hide the cursor at beginning
        CursorOn.SetActive(false);
        CursorOff.SetActive(false);
	}

    private void Update()
    {
        if (GazeManager.Instance == null)
        {
            return;
        }
        UpdateGeneralCursorState();
        UpdateHandCursorState();
        UpdateScrollCursorState();
        UpdatePathCursorState();
    }

    private GameObject InstantiatePrefab(GameObject inputPrefab)
    {
        if (inputPrefab == null || Billboard==null)
        {
            return null;
        }
        GameObject spawn = null;
        spawn = Instantiate(inputPrefab);
        spawn.transform.parent = Billboard.transform;
        spawn.transform.localPosition = Vector3.zero;
        spawn.SetActive(false);

        return spawn;
    }

    private void UpdateGeneralCursorState()
    {
        if (CursorOn == null || CursorOff == null)
        {
            return;
        }
        if (GazeManager.Instance.Hit)
        {
            CursorOn.SetActive(true);
            CursorOff.SetActive(false);
        }
        else
        {
            CursorOn.SetActive(false);
            CursorOff.SetActive(true);
        }

        transform.position = GazeManager.Instance.HitPosition;
        transform.up = GazeManager.Instance.HitNormal;
    }

    private void UpdateHandCursorState()
    {
        if (handDetectedCursor == null)
        {
            return;
        }
        handDetectedCursor.SetActive(HandsManager.Instance.HandDetected);
    }

    private void UpdateScrollCursorState()
    {
        if (scrollDetectedCursor == null)
        {
            return;
        }

        scrollDetectedCursor.SetActive(GestureManager.Instance.IsNavigation);
    }

    private void UpdatePathCursorState()
    {
        if (pathDetectedCursor == null)
        {
            return;
        }
        pathDetectedCursor.SetActive(GestureManager.Instance.IsManipulation);
    }
}
