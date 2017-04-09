using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComRef<T>
{
    public delegate T GetTMethod();
    public  GetTMethod _getT;

    private T _ref;

    public T Ref
    {
        get
        {
            if (_ref == null)
            {
                _ref = _getT();
            }
            return _ref;
        }
    }

    public ComRef(GetTMethod _Method)
    {
        _getT = _Method;
    }
}
