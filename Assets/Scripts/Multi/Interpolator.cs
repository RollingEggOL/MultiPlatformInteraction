using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    private const float SmallNumber = 0.0000001f;

    private Vector3 targetPosition;


    [HideInInspector]
    public bool SmoothLerpToTarget = false;
    [HideInInspector]
    public float SmoothPositionLerpRatio = 0.5f;
    [HideInInspector]
    public float PositionPerSecond = 7.0f;

    public bool AnimaitonPosition
    {
        get;
        private set;
    }

    public bool Running
    {
        get
        {
            return (AnimaitonPosition);
        }
    }

    public void SetTargetPosition(Vector3 target)
    {
        //bool wasRunning =
        targetPosition = target;

        float magsq = (targetPosition - transform.position).sqrMagnitude;
        if (magsq > SmallNumber)
        {
            AnimaitonPosition = true;
            enabled = true;
        }
        else
        {
            transform.position = target;
            AnimaitonPosition = false;
        }
    }

    public Vector3 PositionVelocity
    {
        get;
        private set;
    }
    private Vector3 oldPosition = Vector3.zero;

    public void Update()
    {
        bool interpOccuredThisFrame = false;
        if (AnimaitonPosition)
        {
            Vector3 lerpTargetPosition = targetPosition;
            if (SmoothLerpToTarget)
            {
                //Interpolates between the vectors a and b by the interpolant t. The parameter t is clamped to the range [0, 1]
                lerpTargetPosition = Vector3.Lerp(transform.position, lerpTargetPosition, SmoothPositionLerpRatio);
            }

            //
            Vector3 newPositon = NonLinearInterpolateTo(transform.position, lerpTargetPosition, Time.deltaTime, PositionPerSecond);
            //new position is the position calcuated by the targetDirection,and velocity
            //it updated every frame,it will be closer to targetposition every frame.
            //if the newposition is close to targetpositon enough,then the function will be close
            if ((targetPosition - newPositon).sqrMagnitude <= SmallNumber)
            {
                newPositon = targetPosition;
                AnimaitonPosition = false;
            }
            else
            {
                interpOccuredThisFrame = true;
            }

            transform.position = newPositon;

            PositionVelocity = newPositon - oldPosition;
            oldPosition = newPositon;
        }

        if (!interpOccuredThisFrame)
        {
            enabled = false;
        }
    }

    public static Vector3 NonLinearInterpolateTo(Vector3 start,Vector3 target,float deltaTime,float speed)
    {
        if (speed <= 0.0f)
        {
            return target;
        }

        Vector3 distance = (target - start);

        if (distance.sqrMagnitude <= Mathf.Epsilon)
        {
            return target;
        }

        Vector3 deltaMove = distance * Mathf.Clamp(deltaTime * speed, 0, 1);

        return start + deltaMove;
    }

}
