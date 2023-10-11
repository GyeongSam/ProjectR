using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastRoomStartCheck : MonoBehaviour
{
    int _mirrorCount;
    FloesMoveStartScript _script;
    GameObject _floeLights;
    public int MirrorCount
    {
        get { return _mirrorCount; }
        set { _mirrorCount = value; }
    }
    void Start()
    {
        _mirrorCount = 0;
        _script = transform.GetChild(0).GetComponent<FloesMoveStartScript>();
        _floeLights = transform.GetChild(1).gameObject;
    }

    void Update()
    {
        if (_mirrorCount == 6)
        {
            _script.StartMoveFloes();
            _floeLights.SetActive(false);
            GetComponent<LastRoomStartCheck>().enabled = false;
        }
    }
}
