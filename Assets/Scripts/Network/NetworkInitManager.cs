using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInitManager : NetworkBehaviour
{
    public GameObject prefab_Camera_simu;
    public override void OnStartLocalPlayer()
    {
        if (NetworkSpawner.HasStartServer)
        {
            NetworkSpawner.Instance.SpawnPrefab();
        }
        GenerateSimuCamera();
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        StopAllCoroutines();
        StartCoroutine(DisActiveNonLocalPlayer());
    }

    /// <summary>
    ///For OnStartClient is called before OnStartLcoalPlayer
    ///so the parameter IsLocalPlayer will always return false in OnStartClient
    ///so we make a coroutine to detected the IsLocalPlayer in next frame
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisActiveNonLocalPlayer()
    {
        yield return null;
        NetworkInitManager[] Managers = FindObjectsOfType<NetworkInitManager>();
        for (int index = 0; index != Managers.Length; ++index)
        {
            if (!Managers[index].isLocalPlayer)
            {
                //Destroy(Managers[index].gameObject);
                Managers[index].gameObject.SetActive(false);
            }
        }
    }

    GameObject Camera_simu;
    public void GenerateSimuCamera()
    {
        if (isLocalPlayer)
        {
            if (isServer)
            {
                return;
            }
            Camera_simu = Instantiate(prefab_Camera_simu);
        }
    }

    //private void Update()
    //{
    //    Debug.Log("is local player" + isLocalPlayer);
    //}

    public void DestroySimuCamera()
    {
        if (Camera_simu != null)
        {
            Destroy(Camera_simu);
        }
    }
}
