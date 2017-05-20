using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerIndicator : MonoBehaviour {

	private ComRef<Transform> ControllerBtn;
	private LineRenderer _lineRender;

	private void OnEnable()
	{
		ControllerBtn = new ComRef<Transform> (() => {
			return transform.Find ("Model/button/attach");
		});
		if (_lineRender== null) 
		{
			_lineRender=gameObject.AddComponent<LineRenderer> ();
			_lineRender.positionCount = 2;
			_lineRender.startWidth = 0.01f;
			_lineRender.endWidth = 0.01f;

		}
		StartCoroutine (UpdateLinerender());
	}

	private void OnDisable()
	{
		StopAllCoroutines ();
	}

	IEnumerator UpdateLinerender()
	{
		while (true) 
		{
			if (ControllerBtn.Ref == null || _lineRender == null) 
			{
				yield return null;
			}
			break;
		}
		while (true)
		{
			_lineRender.SetPosition (0, transform.position);
			_lineRender.SetPosition (1,transform.position+ transform.forward*3);
			yield return null;
		}
	}
}
