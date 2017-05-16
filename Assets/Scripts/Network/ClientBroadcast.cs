using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientBroadcast : NetworkBehaviour
{

    ComRef<Transform> CameraTransform;

    private void Start()
    {
        CameraTransform = new ComRef<Transform>(() =>
          {
              return Camera.main.transform;
          });
    }

    private void Update()
    {
        if (!NetworkSpawner.HasStartServer)
        {
            return;
        }
        RpcUpdateCameraSimu(CameraTransform.Ref.position, CameraTransform.Ref.rotation);
    }

    public GameObject Camera_simu = null;

    [ClientRpc]
    void RpcUpdateCameraSimu(Vector3 Position,Quaternion Rotation)
    {
        if (!isLocalPlayer || isServer)
        {
            return;
        }
        if (Camera_simu == null)
        {
            Camera_simu = transform.GetComponent<NetworkInitManager>().Camera_simu;
            return;
        }
        if (Camera_simu != null)
        {
            Camera_simu.transform.position = Position;
            Camera_simu.transform.rotation = Rotation;
        }
    }
}
