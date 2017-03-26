using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    const float HorizontalSpeed = 0.01f;
    const float DeepSpeed = 0.01f;
    const float VerticalSpeed = 0.01f;

    Transform _mainCamera=null;
    public Transform Cursor;
    private MeshRenderer _CursorMeshRender;
    LineRenderer _lineRender = null;

    private GameObject _focusGameObject;
    private GameObject _ProcessGameObject;
    private GameObject _ProcessRoot;
    private Vector3 _OriginPosition;
	// Use this for initialization
	void Start ()
    {
        _mainCamera = Camera.main.transform;
        _CursorMeshRender = Cursor.GetComponentInChildren<MeshRenderer>();
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        float x = Input.GetAxis("Horizontal") * HorizontalSpeed;
        float y = 0;
        if (Input.GetKey("q"))
        {
            y = VerticalSpeed;
        }
        else if(Input.GetKey("e"))
        {
            y = -VerticalSpeed;
        }
        float z = Input.GetAxis("Vertical") * DeepSpeed;
        _mainCamera.Translate(x, y, z);

        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out hit))
        {
            _CursorMeshRender.enabled = true;
            Cursor.position = hit.point;
            Cursor.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Debug.Log("hit gameobject" + hit.transform.name);
            _focusGameObject = hit.transform.gameObject;
        }
        else
        {
            _CursorMeshRender.enabled = false;
            Debug.Log("NO GAMEOBJECT HIT");
            _focusGameObject = null;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            if (_focusGameObject != null && _ProcessGameObject==null)
            {
                _ProcessGameObject = _focusGameObject;
                _ProcessRoot = _ProcessGameObject.transform.parent.gameObject;
                _ProcessGameObject.transform.SetParent(null);
                _ProcessRoot.SetActive(false);
                _OriginPosition = _ProcessGameObject.transform.position;
                _focusGameObject.transform.Translate(_mainCamera.forward*-1.0f);
            }
        }
        if (Input.GetKeyDown("r"))
        {
            if (_ProcessGameObject != null && _OriginPosition!=null)
            {
                _ProcessGameObject.transform.position = _OriginPosition;
                _ProcessGameObject.transform.SetParent(_ProcessRoot.transform);
                _ProcessRoot.SetActive(true);
            }
            _ProcessGameObject = null;
        }

    }
}
