using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSpawner : NetworkBehaviour
{
    public GameObject prefab_UI;
    public GameObject prefab_Brat;
    private Transform generalUI;

    public static bool HasStartServer;

    public static NetworkSpawner _Instance;
    public static NetworkSpawner Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<NetworkSpawner>();
            }
            return _Instance;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        HasStartServer = true;
        Debug.Log("Call in start server");

    }

    public void SpawnPrefab()
    {
        generalUI = GameObject.Find("GeneralUI").transform;

        GameObject Brat = Instantiate(prefab_Brat);
        ClientScene.RegisterPrefab(prefab_Brat);
        NetworkServer.Spawn(Brat);

        GameObject UI = Instantiate(prefab_UI, generalUI);
        UI.transform.localScale = Vector3.one;
        UI.transform.localPosition = Vector3.zero;
        UI.transform.localRotation = Quaternion.Euler(Vector3.zero);
        ClientScene.RegisterPrefab(prefab_UI);
        NetworkServer.Spawn(UI);
    }
}
