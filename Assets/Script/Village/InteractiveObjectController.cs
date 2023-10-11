using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectController : MonoBehaviour
{
    bool _isActive = false;
    bool _done = false;

    Define.Condition _condition;

    Define.PlayerColor _color;

    [SerializeField]
    Define.Condition _requiredCondition;

    [SerializeField]
    Define.PlayerColor _requiredColor;

    GameObject Camera;


    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }
    public bool Done
    {
        get { return _done; }
        set { _done = value; }
    }
    public Define.Condition Condition
    {
        get { return _condition; }
        set { _condition = value; }
    }

    public Define.Condition RequiredCondition
    {
        get { return _requiredCondition; }
    }

    public Define.PlayerColor Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public Define.PlayerColor RequiredColor
    {
        get { return _requiredColor; }
    }


    void Start()
    {
        _condition = Define.Condition.None;
        _color = Define.PlayerColor.None;
    }
}
