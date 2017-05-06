using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSpawner : NetworkBehaviour
{
    public GameObject prefab_Manager;
    public GameObject prefab_UI;
    public GameObject prefab_Brat;
    private Transform generalUI;

    public override void OnStartServer()
    {
        base.OnStartServer();
        generalUI = GameObject.Find("GeneralUI").transform;

        GameObject Brat = Instantiate(prefab_Brat);
        ClientScene.RegisterPrefab(prefab_Brat);
        NetworkServer.Spawn(Brat);

        GameObject Manager = Instantiate(prefab_Manager);
        ClientScene.RegisterPrefab(prefab_Manager);
        NetworkServer.Spawn(Manager);

        GameObject UI = Instantiate(prefab_UI,generalUI);
        UI.transform.localScale = Vector3.one;
        UI.transform.localPosition = Vector3.zero;
        UI.transform.localRotation = Quaternion.Euler(Vector3.zero);
        ClientScene.RegisterPrefab(prefab_UI);

    }
}
