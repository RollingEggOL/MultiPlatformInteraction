using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum frustumDirection
{
    frustumLeft,
    frustumRight,
    frustumBottom,
    frustumTop
}
public class Tagalong : MonoBehaviour
{
    [Tooltip("The distance in meters from the camera for the Tagalong to seek when updating its position")]
    public float TagalongDistance = 2.0f;

    private BoxCollider taralongCollider;
    private Interpolator interpolatpor;

    private Plane[] frustumPlanes;
    private ComRef<Transform> cameraTransform;

    private void Start()
    {
        taralongCollider = GetComponent<BoxCollider>();
        if (!taralongCollider)
        {
            enabled = false;
            Debug.LogError("NO_BOXCOLLIDER_ON_TAGALONG_OBJ");
        }

        cameraTransform = new ComRef<Transform>(()=> 
        {
            return Camera.main.transform;
        });

        interpolatpor = gameObject.AddComponent<Interpolator>();
        interpolatpor.SmoothLerpToTarget = true;
        interpolatpor.SmoothPositionLerpRatio = 0.75f;
        interpolatpor.PositionPerSecond = 3.0f;

        gameObject.AddComponent<Billboard>();
    }


    private void FixedUpdate()
    {
        //Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Vector3 tagalongTargetPosition;
        if (CalculateTagalongTargetPosition(transform.position, out tagalongTargetPosition))
        {
            interpolatpor.SetTargetPosition(tagalongTargetPosition);
        }
    }

    private bool CalculateTagalongTargetPosition(Vector3 fromPosition,out Vector3 targetPosition)
    {
        bool needsMove = !GeometryUtility.TestPlanesAABB(frustumPlanes, taralongCollider.bounds);

        //To set the default toPosition
        targetPosition = cameraTransform.Ref.position + cameraTransform.Ref.forward * TagalongDistance;

        if (!needsMove)
        {
            return false;
        }

        Ray ray = new Ray(targetPosition, Vector3.zero);
        Plane plane = new Plane();
        float distanceOffset;

        //GetDistanceToPoint:
        //returned is positive if the point is on the side of the plane into which the plane's normal is facing, and negative otherwise.
        //the plane's normal direction is to point the origin,so the left plane of frustum is point right.
        //so if the point is right to the frustum(in the camera frustum),it should return the positive value
        //otherwise it means the point is left to the frustum(out),so it need to move right
        bool moveRight = frustumPlanes[(int)frustumDirection.frustumLeft].GetDistanceToPoint(fromPosition) < 0;
        bool moveLeft = frustumPlanes[(int)frustumDirection.frustumRight].GetDistanceToPoint(fromPosition) < 0;
        if (moveRight)
        {
            //The rayorigin is topositon(in the camera's forward direction,so it is in the frustum)
            //we need the make the ray to raycast a plane
            //so if we need to moveright,the plane should be the left plane
            //and the ray's direction should be the left
            plane = frustumPlanes[(int)frustumDirection.frustumLeft];
            ray.direction = -cameraTransform.Ref.right;
        }
        else if (moveLeft)
        {
            plane = frustumPlanes[(int)frustumDirection.frustumRight];
            ray.direction = cameraTransform.Ref.right;
        }
        if (moveRight || moveLeft)
        {
            //Plane.Raycast:
            //Intersects a ray with the plane
            plane.Raycast(ray, out distanceOffset);

            //get the ray and plane's intersection point,this point's x should be the targetposion's x
            targetPosition.x = ray.GetPoint(distanceOffset).x;
        }

        bool moveDown = frustumPlanes[(int)frustumDirection.frustumTop].GetDistanceToPoint(fromPosition) < 0;
        bool moveUp = frustumPlanes[(int)frustumDirection.frustumBottom].GetDistanceToPoint(fromPosition) < 0;

        if (moveDown)
        {
            plane = frustumPlanes[(int)frustumDirection.frustumTop];
            ray.direction = cameraTransform.Ref.up;
        }
        else if (moveUp)
        {
            plane = frustumPlanes[(int)frustumDirection.frustumBottom];
            ray.direction = -cameraTransform.Ref.up;
        }
        if (moveUp || moveDown)
        {
            plane.Raycast(ray, out distanceOffset);
            targetPosition.y = ray.GetPoint(distanceOffset).y;
        }

        ray = new Ray(cameraTransform.Ref.position, targetPosition - cameraTransform.Ref.position);
        targetPosition = ray.GetPoint(TagalongDistance);

        return needsMove;
    }
}
