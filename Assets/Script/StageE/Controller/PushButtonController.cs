using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PushButtonController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Define.PlayerColor RequiredColor;

    bool _active = false;
    public bool Active
    {
        get { return _active; }
        set { _active = value; }
    }

    bool _on = false;
    public bool On
    {
        get { return _on; }
        set { _on = value; }
    }

    bool _done = false;

    public bool Done
    {
        get { return _done; }
        set { _done = value; }
    }
    TimerButtonController _timer;

    GameObject _cover;

    Vector3 _start;
    void Start()
    {
        _start = transform.position - new Vector3(0, 1.21f, 0.42274f);
        _timer = gameObject.transform.parent.parent.gameObject.GetComponent<TimerButtonController>();
        _cover = transform.parent.GetChild(1).gameObject;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if(!_done && !_active && Physics.Raycast(_start, Vector3.up, out hit, 1.0f, (1 << 3)))
        {
            Define.PlayerColor nplayer = hit.transform.gameObject.GetComponent<PlayerControllerE>().PlayerColor;
            if (nplayer == RequiredColor && PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().PlayerColor == nplayer)
            {
                _active = true;
                photonView.RPC("ButtonOn", RpcTarget.AllBufferedViaServer, null);
            }
        }

        else if(_active && !Physics.Raycast(_start, Vector3.up, 1.0f, (1 << 3)))
        {
            _active = false;
        }
    }

    public void Open()
    {
        _cover.transform.position += new Vector3(0, 1.85f, 0);
    }

    public void Close()
    {
        _cover.transform.position -= new Vector3(0, 1.85f, 0);
    }

    [PunRPC]
    void ButtonOn()
    {
        if (_timer.Timer == null) _timer.StartTimer();
        _on = true;
        Close();
    }
}
