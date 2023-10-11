using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField]
    GameObject _door;
    Vector3 _size = new Vector3(5.75f, 1.0f, 6.145f);
    
    void FixedUpdate()
    {
        if (Physics.CheckBox(transform.position, _size, Quaternion.identity, (1 << 3)))
            _door.SetActive(true);
        else
            _door.SetActive(false);
    }
}
