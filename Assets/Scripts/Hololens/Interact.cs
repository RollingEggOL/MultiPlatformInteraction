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

    public static GameObject _selectedGameObject;

    private Vector3 originPositionOfSelectedObj;
    private Quaternion originRotationOfSelectedObj;

    public void InitPanel()
    {
        if (_Panel != null)
        {
            return;
        }

        _Panel = new ComRef<GameObject>(() =>
        {
            return GameObject.Find("UI/Panel");
        });

        _Panel.Ref.SetActive(false);
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
        //Update the materials which need to be handled,and set the slider according to the materials
        UIManager.Instance._HandledMaterials = defaultMaterials;
        UIManager.Instance.InitSliderValue();

        bool _cancelSelected = _selectedGameObject == gameObject;      
        SwitchGameobject(_cancelSelected);
        if (_cancelSelected)
        {
            CancelProcess();
        }
        else
        {
            SelectProcess();
        }
    }

    /// <summary>
    /// if the currenyly select gameobject has been selected,then cancel the select mode
    /// which the select object will be closer to player,and others gameobjects will be hided
    /// </summary>
    void SwitchGameobject(bool cancel)
    {
        for (int index = 0; index != InteractibleObject.Length; ++index)
        {
            Interact temp = InteractibleObject[index];
            if (temp != this)
            {
                temp.gameObject.SetActive(cancel);
            }
            else
            {
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

    private void SelectProcess()
    {
        _selectedGameObject = gameObject;
        _Panel.Ref.SetActive(true);
        //the GUI of Panel actually don't need switch recognizer,so this can use for rotation
    }

    private void CancelProcess()
    {
        _selectedGameObject = null;
        _Panel.Ref.SetActive(false);
    }
}
