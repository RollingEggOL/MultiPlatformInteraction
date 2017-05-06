using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiNetworkManager : NetworkManager
{
    public Text NetworkStateText;



    public override void OnStartHost()
    {
        Debug.Log("Connect the host");
        base.OnStartHost();
        NetworkStateText.text = "Host is On";
        //FindObjectOfType<DirectionIndicator>().InitTargetGameobject();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("Disconnect the host");
        NetworkStateText.text = "Host is Off";
    }
}
