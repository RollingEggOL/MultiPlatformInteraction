using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle the gaze message
/// </summary>
public class Interact : MonoBehaviour
{
    public Material[] defaultMaterials;

    //make static in case to keep only init once
    private static Interact[] InteractibleObject;
    private static ComRef<GameObject> _Panel;
    private static GameObject _Parent;

    private static GameObject _selectedGameObject;
    public static GameObject SelectedGameObject
    {
        get
        {
            return _selectedGameObject;
        }
        private set
        {
            _selectedGameObject = value;
            
            DirectionIndicator.Instance.TargetGameobject = _selectedGameObject??_Parent;
        }
    }



    public void InitPanel()
    {
        _Panel = new ComRef<GameObject>(() =>
        {
            if (SpeechManager.Instance.IsNetworkScene)
            {
                return GameObject.Find("GeneralUI/UI(Clone)/Panel");
            }
            else
            {
                return GameObject.Find("UI/Panel");
            }
        });
        if (_Panel.Ref != null)
        {
            _Panel.Ref.SetActive(false);
        }
    }

    private void InitInteractibleObject()
    {
        if (InteractibleObject != null)
        {
            return;
        }
        //Find all brother
        Transform _parent = transform.parent;

        InteractibleObject = new Interact[_parent.childCount];
        for (int index = 0; index != _parent.childCount; ++index)
        {
            InteractibleObject[index] = _parent.GetChild(index).GetComponent<Interact>();
        }
        if (InteractibleObject.Length != _parent.childCount)
        {
            Debug.LogError("INTERACTIBLEOBJECT_INIT_FAILED");
        }
    }

    private void Start()
    {
        defaultMaterials = GetComponent<Renderer>().materials;

        if (_Parent == null)
        {
            _Parent = transform.parent.gameObject;
        }

        InitPanel();
        InitInteractibleObject();
    }

    void GazeEnter()
    {
        if (defaultMaterials == null)
        {
            return;
        }
        for (int i = 0; i != defaultMaterials.Length; ++i)
        {
            defaultMaterials[i].SetFloat("_Highlight", 0.25f);
        }
    }

    void GazeExit()
    {
        if (defaultMaterials == null)
        {
            return;
        }
        for (int i = 0; i != defaultMaterials.Length; ++i)
        {
            defaultMaterials[i].SetFloat("_Highlight", 0f);
        }
    }

    void OnSelect()
    {

        bool _cancelSelected = SelectedGameObject == gameObject;

        var ClientBroadcasts = FindObjectsOfType<ClientBroadcast>();
        foreach (var client in ClientBroadcasts)
        {
            if (client.isServer)
            {
                client.RpcSwitchFocusGameobject(name, _cancelSelected);
            }
        }

        if (_cancelSelected)
        {
            CancelProcess();
        }
        else
        {
            SelectProcess();
        }
    }

    private void SelectProcess()
    {
        SelectedGameObject = gameObject;
        _Panel.Ref.SetActive(true);
        //the GUI of Panel actually don't need switch recognizer,so this can use for rotation
    }

    private void CancelProcess()
    {
        SelectedGameObject = null;
        _Panel.Ref.SetActive(false);
    }
}
