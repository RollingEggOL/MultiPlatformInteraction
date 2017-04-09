using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle the gaze message
/// </summary>
public class Interact : MonoBehaviour
{
    private Material[] defaultMaterials;
    private Interact[] InteractibleObject;

    private static Interact _selectedGameObject;

    private void Start()
    {
        defaultMaterials = GetComponent<Renderer>().materials;

        //Find all brother
        Transform _parent= transform.parent;
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

    void GazeEnter()
    {
        for (int i = 0; i != defaultMaterials.Length; ++i)
        {
            defaultMaterials[i].SetFloat("_Highlight", 0.25f);
        }
    }

    void GazeExit()
    {
        for (int i = 0; i != defaultMaterials.Length; ++i)
        {
            defaultMaterials[i].SetFloat("_Highlight", 0f);
        }
    }

    void OnSelect()
    {
        SwitchGameobject();
    }

    /// <summary>
    /// if the currenyly select gameobject has been selected,then cancel the select mode
    /// which the select object will be closer to player,and others gameobjects will be hided
    /// </summary>
    void SwitchGameobject()
    {
        bool _cancelSelected = _selectedGameObject == this;
        for (int index = 0; index != InteractibleObject.Length; ++index)
        {
            Interact temp = InteractibleObject[index];
            if (temp != this)
            {
                temp.gameObject.SetActive(_cancelSelected);
            }
            else
            {
                Vector3 direction = Camera.main.transform.forward;
                float factor = _selectedGameObject == this ? 1.0f : -1.0f;
                temp.transform.position +=direction*factor;
            }
        }

        //Update SelectGameobject
        if (_cancelSelected)
        {
            _selectedGameObject = null;
        }
        else
        {
            _selectedGameObject = this;
        }
    }
}
