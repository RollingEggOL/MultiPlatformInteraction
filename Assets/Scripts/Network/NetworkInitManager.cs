using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInitManager : NetworkBehaviour
{
    public GameObject prefab_Camera_simu;
    public override void OnStartLocalPlayer()
    {
        Debug.Log("Local player");
        if (NetworkSpawner.HasStartServer)
        {
            NetworkSpawner.Instance.SpawnPrefab();
        }
        GenerateSimuCamera();
    }

    GameObject Camera_simu;
    public void GenerateSimuCamera()
    {
        Debug.Log("Enter ClientRPC");
        if (isLocalPlayer)
        {
            Debug.Log("Enter instantiate");
            if (isServer)
            {
                return;
            }
            Camera_simu = Instantiate(prefab_Camera_simu);
            if (Camera_simu == null)
            {
                Debug.Log("Instantiate fail");
            }
        }
        Debug.Log("Exit instantiate");
    }

    public void DestroySimuCamera()
    {
        Debug.Log("Enter destroy camera");
        if (Camera_simu != null)
        {
            Destroy(Camera_simu);
        }
    }
}
