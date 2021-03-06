﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// the SliderType:which value is the variant in shader
/// </summary>
public class SliderType
{
    public const string _Red = "_Red";
    public const string _Blue = "_Blue";
    public const string _Green = "_Green";
}

public class UIManager : Singleton<UIManager>
{

    private Slider slider_Red;
    private Slider slider_Blue;
    private Slider slider_Green;
    private Toggle toggle_Tagalong;
    private Toggle toggle_Manipultation;
    private Tagalong _tagalong;


    [HideInInspector]
    public Material[] _HandledMaterials;

    private void Start()
    {
        _tagalong = transform.Find("Panel").GetComponent<Tagalong>();

        slider_Red = transform.Find("Panel/Red").GetComponent<Slider>();
        slider_Blue = transform.Find("Panel/Blue").GetComponent<Slider>();
        slider_Green = transform.Find("Panel/Green").GetComponent<Slider>();
        toggle_Tagalong = transform.Find("Panel/Tagalong").GetComponent<Toggle>();
        toggle_Manipultation = transform.Find("Panel/Manipulation").GetComponent<Toggle>();
    }



    public void SliderValueChangedListener_Red()
    {
        if (_HandledMaterials == null)
        {
            return;
        }

        SetMaterials(SliderType._Red);
    }

    public void SliderValueChangedListener_Blue()
    {
        if (_HandledMaterials == null)
        {
            return;
        }

        SetMaterials(SliderType._Blue);
    }

    public void SliderValueChangedListener_Green()
    {
        if (_HandledMaterials == null)
        {
            return;
        }

        SetMaterials(SliderType._Green);
    }

    public void ToggleValueChangedListener_Tagalong()
    {
        if (toggle_Tagalong.isOn)
        {
            _tagalong.enabled = true;
        }
        else
        {
            _tagalong.enabled = false;
        }
    }

    public void ToggleValueChangedListener_Manipulation()
    {
        if (toggle_Manipultation.isOn)
        {
            GestureManager.Instance.SwitchRecognizer(GestureManager.Instance.ManipulationRecognizer);
        }
        else
        {
            GestureManager.Instance.SwitchRecognizer(GestureManager.Instance.NavigationRecognizer);
        }
    }

    public void SetManipulationToggleValue(bool on)
    {
        toggle_Manipultation.isOn = on;
        ToggleValueChangedListener_Manipulation();
    }


    /// <summary>
    /// Set the Materials's rgb value
    /// </summary>
    /// <param name="_sliderType"> the slider type</param>
    private void SetMaterials(string _sliderType)
    {
        float value = 0;
        switch (_sliderType)
        {
            case SliderType._Red:
                value = slider_Red.value;
                break;
            case SliderType._Green:
                value = slider_Green.value;
                break;
            case SliderType._Blue:
                value = slider_Blue.value;
                break;
        }

        ClientBroadcast[] clients = FindObjectsOfType<ClientBroadcast>();
        foreach (var client in clients)
        {
            if (client.isServer)
            {
                client.RpcSetColor(_sliderType, value);
            }
        }
    }

    /// <summary>
    /// Init the value of slider to keep the same with rgb value of the selectedGameobject's materials
    /// </summary>
    public void InitSliderValue()
    {
        if (_HandledMaterials == null)
        {
            Debug.LogError("NO_MATERIALS_BEFOR_INIT");
            return;
        }
        float _redvalue = _HandledMaterials[0].GetFloat(SliderType._Red);
        float _greenValue = _HandledMaterials[0].GetFloat(SliderType._Green);
        float _blueValue = _HandledMaterials[0].GetFloat(SliderType._Blue);

        slider_Red.value = _redvalue;
        slider_Green.value = _greenValue;
        slider_Blue.value = _blueValue;
    }
}
