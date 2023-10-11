using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    Define.MoveDirection _moveDirection;
    public Define.MoveDirection MoveDirection
    {
        get { return _moveDirection; }
    }
}
