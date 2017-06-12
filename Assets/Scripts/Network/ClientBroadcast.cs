using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientBroadcast : NetworkBehaviour
{

    private ComRef<Transform> CameraTransform;
    private Vector3 originPositionOfSelectedObj;
    private Quaternion originRotationOfSelectedObj;

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
        if (isServer)
        {
            RpcUpdateCameraSimu(CameraTransform.Ref.position, CameraTransform.Ref.rotation);
        }
    }

    [HideInInspector]
    public GameObject Camera_simu = null;
    private GameObject[] InteractObjs;

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


    private Camera SimuCameraComponent;

    [ClientRpc]
    public void RpcSwitchCamera(bool follow)
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
            Debug.Log("Enter switch camera");
            if (follow)
            {
                if (SimuCameraComponent == null)
                {
                    SimuCameraComponent=Camera_simu.AddComponent<Camera>();
                    SimuCameraComponent.depth = 2;
                }
                else
                {
                    SimuCameraComponent.enabled = true;
                }
            }
            else
            {
                if (SimuCameraComponent == null)
                {
                    SimuCameraComponent = Camera_simu.AddComponent<Camera>();
                    SimuCameraComponent.enabled = false;
                }
                else
                {
                    SimuCameraComponent.enabled = false;
                }

            }
        }
    }


    [ClientRpc]
    public void RpcSwitchFocusGameobject(string SelectedObjName,bool cancel)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (InteractObjs == null)
        {
            var Interacts = FindObjectsOfType<Interact>();
            InteractObjs = new GameObject[Interacts.Length];
            for (int index = 0; index != Interacts.Length; ++index)
            {
                InteractObjs[index] = Interacts[index].gameObject;
            }
        }
        for (int index = 0; index != InteractObjs.Length; ++index)
        {
            GameObject temp = InteractObjs[index];
            if (temp.name != SelectedObjName)
            {
                temp.SetActive(cancel);
            }
            else
            {
                UIManager.Instance._HandledMaterials = temp.GetComponent<Interact>().defaultMaterials;
                UIManager.Instance.InitSliderValue();
                if (!cancel)
                {
                    Vector3 direction = Camera.main.transform.forward;
                    originPositionOfSelectedObj = temp.transform.position;
                    originRotationOfSelectedObj = temp.transform.rotation;
                    temp.transform.position -= direction;
                }
                else
                {
                    temp.transform.position = originPositionOfSelectedObj;
                    temp.transform.rotation = originRotationOfSelectedObj;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcSetColor(string _sliderType,float value)
    {
        Material[] materials = UIManager.Instance._HandledMaterials;
        if (!isLocalPlayer)
        {
            return;
        }
        if (materials == null || materials.Length == 0)
        {
            Debug.Log("No materials");
        }
        for (int index = 0; index != materials.Length; ++index)
        {
            materials[index].SetFloat(_sliderType, value);
        }
    }
}
