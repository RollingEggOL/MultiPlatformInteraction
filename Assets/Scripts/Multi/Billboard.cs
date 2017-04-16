using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PivotAxis
{
    Free,
    X,
    Y
}

public class Billboard : MonoBehaviour
{
    public PivotAxis PivotAxis = PivotAxis.Free;

    private ComRef<Transform> cameraTransfrom;

    public Quaternion DefaultRotation
    {
        get;
        private set;
    }

    private void Awake()
    {
        DefaultRotation = transform.rotation;
    }

    private void Start()
    {
        cameraTransfrom = new ComRef<Transform>(()=> 
        {
            return Camera.main.transform;
        });
    }

    private void FixedUpdate()
    {
        Vector3 directionToTarget = cameraTransfrom.Ref.position - transform.position;
        switch (PivotAxis)
        {
            case PivotAxis.X:
                directionToTarget.x = transform.position.x;
                break;
            case PivotAxis.Y:
                directionToTarget.y = transform.position.y;
                break;
            case PivotAxis.Free:
            default:
                break;
        }

        gameObject.transform.rotation = Quaternion.LookRotation(-directionToTarget) * DefaultRotation;
    }
}
