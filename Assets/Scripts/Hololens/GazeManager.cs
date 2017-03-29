using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeManager : Singleton<GazeManager>
{
    [Tooltip("The max gaze distance for calculationg a hit")]
    public float MaxGazeDistance = 5.0f;

    [Tooltip("Select the layers raycast should target")]
    public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

    /// <summary>
    /// Whether the raycast hit something or not
    /// </summary>
    public bool Hit
    {
        get;
        private set;
    }

    public RaycastHit HitInfo
    {
        get;
        private set;
    }

    /// <summary>
    /// Hit Position
    /// </summary>
    public Vector3 HitPosition
    {
        get;
        private set;
    }

    /// <summary>
    /// Hit Normal
    /// </summary>
    public Vector3 HitNormal
    {
        get;
        private set;
    }

    private Vector3 gazeOrigin;
    private Vector3 gazeDirection;

    private void Update()
    {
        gazeOrigin = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;

        //there is no stable

        UpdateRaycast();
    }

    private void UpdateRaycast()
    {
        RaycastHit hitInfo;
        Hit = Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo, MaxGazeDistance, RaycastLayerMask);
        HitInfo = hitInfo;

        //if hit something then the Hitposition and HitNormal will according to the hit data 
        //else the Hitposition and HitNormal will according to the camera's rotation and MaxGazeDistance 
        if (Hit)
        {
            //minus to make sure that the cursor will always be out the model
            HitPosition = hitInfo.point-gazeDirection*0.1f;
            HitNormal = hitInfo.normal;
        }
        else
        {
            HitPosition = gazeOrigin + (gazeDirection * MaxGazeDistance);
            HitNormal = gazeDirection;
        }
    }
}
