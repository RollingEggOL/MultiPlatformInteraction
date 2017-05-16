using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : Singleton<DirectionIndicator>
{
    [Tooltip("The cursor which the Indicator should follow")]
    public GameObject Cursor;

    [Tooltip("Model of the direction indicator")]
    public GameObject DirectionIndicatorObject;

    [Tooltip("The width of allowable space of the screen")]
    [Range(0, 0.3f)]
    public float ScreenframeWidth = 0.1f;

    [Tooltip("The distance between Cursor and indicator")]
    public float DistanceFromCursor = 0.3f;

    private ComRef<Transform> cameraTransform;

    private Quaternion directionIndicatorDefaultRotation = Quaternion.identity;

    private Collider _targetGameObjectCollider;
    private GameObject _targetGameobject;
    public GameObject TargetGameobject
    {
        get
        {
            return _targetGameobject;
        }
        set
        {
            if (value == null)
            {
                return;
            }
            _targetGameobject = value;
            _targetGameObjectCollider = _targetGameobject.GetComponent<Collider>();
        }
    }

    private void Awake()
    {
        if (DirectionIndicatorObject == null)
        {
            return;
        }

        DirectionIndicatorObject = Instantiate(DirectionIndicatorObject);
        DirectionIndicatorObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        InitTargetGameobject();

        cameraTransform = new ComRef<Transform>(() =>
          {
              return Camera.main.transform;
          });

    }

    private void InitTargetGameobject()
    {
        if (SpeechManager.Instance.IsNetworkScene)
        {
            TargetGameobject = GameObject.Find("Brat_Network(Clone)");
        }
        else
        {
            TargetGameobject = GameObject.Find("Brat");
        }
    }


    private void Update()
    {
        if (DirectionIndicatorObject == null || Cursor==null)
        {
            Debug.LogError("NO_INDICATOR || NO CURSOR");
            return;
        }

        if (TargetGameobject == null)
        {
            Debug.Log("NO_TARGET");
            InitTargetGameobject();
            return;
        }
        //Direction from the camera to TargetGameObject
        Vector3 camToTargetDirection = TargetGameobject.transform.position - cameraTransform.Ref.position;
        camToTargetDirection.Normalize();

        DirectionIndicatorObject.SetActive(!IsTargetVisible);
        if (DirectionIndicatorObject.activeSelf)
        {
            Vector3 position;
            Quaternion rotation;
            GetDirectionIndicatorPositionAndRotation(camToTargetDirection,out position,out rotation);
            DirectionIndicatorObject.transform.position = position;
            DirectionIndicatorObject.transform.rotation = rotation;
        }
    }

    private bool IsTargetVisible
    {
        get
        {
            Plane[] CamerafrustumPlanes= GeometryUtility.CalculateFrustumPlanes(Camera.main); ;
            bool _isVisible = GeometryUtility.TestPlanesAABB(CamerafrustumPlanes, _targetGameObjectCollider.bounds);
            return _isVisible;

            //This method only fit small gameobject
            //Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(TargetGameobject.transform.position);
            //Debug.Log("Target viewport is" + targetViewportPosition);
            //return (targetViewportPosition.x > ScreenframeWidth && targetViewportPosition.x < 1 - ScreenframeWidth &&
            //        targetViewportPosition.y > ScreenframeWidth && targetViewportPosition.y < 1 - ScreenframeWidth &&
            //        targetViewportPosition.z > 0);
        }
    }


    private void GetDirectionIndicatorPositionAndRotation(Vector3 camToObjectDirection,out Vector3 position,out Quaternion rotation)
    {
        Vector3 origin = Cursor.transform.position;

        Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * cameraTransform.Ref.forward);
        cursorIndicatorDirection.Normalize();

        //This will only happen when the target is in the camera's directly backward
        //To avoid the indicator be overlapped on the cursor,set the direction
        if (cursorIndicatorDirection == Vector3.zero)
        {
            cursorIndicatorDirection = Camera.main.transform.right;
        }

        position = origin + cursorIndicatorDirection * DistanceFromCursor;
        rotation = Quaternion.LookRotation(cameraTransform.Ref.forward, cursorIndicatorDirection) * directionIndicatorDefaultRotation;
    }

    private void OnDestroy()
    {
        if (DirectionIndicatorObject != null)
        {
            Destroy(DirectionIndicatorObject);
        }
    }
}
