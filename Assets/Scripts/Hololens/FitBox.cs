using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class FitBox : MonoBehaviour
{
    [Tooltip("The Gameobject to show after click the fitbox")]
    public GameObject HololensCollection;

    [Tooltip("whether the gameobject's position should relative to where the fixbox was dismiseed")]
    public bool MoveCollectionOnDismiss = true;

    private Vector3 CollectionDefaultPosition;

    private float Distance = 2.0f;

    private GestureRecognizer recognizer;

    private void Awake()
    {
        if (HololensCollection)
        {
            CollectionDefaultPosition = HololensCollection.transform.localPosition;
            HololensCollection.SetActive(false);
        }
#if UNITY_WSA_10_0
        //Set gesture listener
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += ((source, tapCount, Ray) => 
        {
            DismissFitBox();
        });
        recognizer.StartCapturingGestures();
#endif
    }

    private void DismissFitBox()
    {
        recognizer.CancelGestures();
        recognizer.StopCapturingGestures();
        recognizer.Dispose();
        recognizer = null;

        if (HololensCollection)
        {
            HololensCollection.SetActive(true);
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

    private void Update()
    {
    }

    private void LateUpdate()
    {
        Transform CameraTransform = Camera.main.transform;
        Transform FitBoxTranform=gameObject.transform;
        FitBoxTranform.position = CameraTransform.position + (CameraTransform.forward * Distance);
        FitBoxTranform.rotation = Quaternion.LookRotation(-CameraTransform.forward, -CameraTransform.up);

    }
}
