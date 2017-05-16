using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
/// <summary>
/// 1.In the LateUpdate we set the fitbox's position by camera's rotation and distance we set(defalut be 2.0f);
/// 2.We set the Tap Gesture's callback,which is Dismissfitbox
/// 3.In Dismissbox function,we will hide fitbox and if we set "MoveCollectionOnDismiss" to be true,the model's positon
/// will be set according it's offset position from camera at beginning,and the camera's rotation the time we tap the fitbox
/// </summary>
public class FitBox : Singleton<FitBox>
{

    [Tooltip("whether the gameobject's position should relative to where the fixbox was dismiseed")]
    public bool MoveCollectionOnDismiss = true;

    private Vector3 CollectionDefaultPosition;

    private float Distance = 2.0f;

    private GestureRecognizer recognizer;
    private GameObject HololensCollection;

    private void Start()
    {
        if (SpeechManager.Instance.IsNetworkScene)
        {
            HololensCollection = GameObject.Find("Brat_Network(Clone)");
        }
        else
        {
            HololensCollection = GameObject.Find("Brat");
        }
        CollectionDefaultPosition = HololensCollection.transform.localPosition;
        HololensCollection.SetActive(false);
        DirectionIndicator.Instance.enabled = false;
        //To hide the panel
        HololensCollection.GetComponentInChildren<Interact>().InitPanel();

#if UNITY_WSA_10_0
        //Set gesture listener
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += ((source, tapCount, Ray) => 
        {
            DismissFitBox();
        });
        recognizer.StartCapturingGestures();
#endif

#if UNITY_EDITOR
        DismissFitBox();
#endif
    }

    private void DismissFitBox()
    {
        if (recognizer != null)
        {
            recognizer.CancelGestures();
            recognizer.StopCapturingGestures();
            recognizer.Dispose();
            recognizer = null;
        }

        if (HololensCollection)
        {
            HololensCollection.SetActive(true);
            DirectionIndicator.Instance.enabled = true;

            if (MoveCollectionOnDismiss)
            {
                Quaternion camQueat = Camera.main.transform.localRotation;

                //to ignore the picth,we should set the rotation around x axis to zero;
                camQueat.x = 0;

                //we do not want the collection be too high 
                Vector3 newPosition = camQueat * CollectionDefaultPosition;
                newPosition.y = CollectionDefaultPosition.y;

                HololensCollection.transform.position = Camera.main.transform.position + newPosition;
                
                //make the collection face the user
                Quaternion toQuat = Camera.main.transform.localRotation * HololensCollection.transform.rotation;
                toQuat.x = 0;
                toQuat.z = 0;
                HololensCollection.transform.rotation = toQuat;

                //Destroty the Fitbox
                Destroy(gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        Transform CameraTransform = Camera.main.transform;
        Transform FitBoxTranform=gameObject.transform;
        FitBoxTranform.position = CameraTransform.position + (CameraTransform.forward * Distance);
        FitBoxTranform.rotation = Quaternion.LookRotation(-CameraTransform.forward, -CameraTransform.up);

    }
}
