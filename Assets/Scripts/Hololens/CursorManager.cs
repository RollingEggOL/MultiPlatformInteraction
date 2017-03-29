using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

    [Tooltip("The cursor when it hits some gameobjects")]
    public GameObject CursorOn;

    [Tooltip("The cursor when it doesn't hit any gameobjects ")]
    public GameObject CursorOff;

	void Start ()
    {
        if (CursorOn == null || CursorOff == null)
        {
            Debug.LogError("THE CURSOR IS NOT SET");
            return;
        }

        //hide the cursor at beginning
        CursorOn.SetActive(false);
        CursorOff.SetActive(false);
	}

    private void Update()
    {
        if (GazeManager.Instance == null || CursorOn == null || CursorOff == null)
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
}
