using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject _skeleton;
    GameObject _door;
    GameObject _skull;

    [SerializeField]
    Define.PlayerColor _requiredColor;
    MeshRenderer _meshRenderer;

    public Define.PlayerColor RequiredColor
    {
        get { return _requiredColor; }
        set { _requiredColor = value; }
    }

    public MeshRenderer MeshRenderer
    {
        get { return _meshRenderer; }
        set { _meshRenderer = value; }
    }

    MeshRenderer _meshRendererSkull;
    public MeshRenderer MeshRendererSkull
    {
        get { return _meshRendererSkull; }
        set { _meshRendererSkull = value; }
    }

    bool _active = false;

    public bool Active
    {
        get { return _active; }
        set { _active = value; }
    }

    bool _started = false;

    public bool Started
    {
        get { return _started; }
        set { _started = value; }
    }

    bool _done = false;

    public bool Done
    {
        get { return _done; }
        set { _done = value; }
    }

    bool _closed = false;

    Vector3 _size = new Vector3(1.9f, 1.5f, 3.8f);
    Vector3 _center;
    void Start()
    {
        _center = transform.position + new Vector3(0, 1.5f, 0);
        _skeleton = transform.GetChild(0).gameObject;
        _door = transform.GetChild(2).gameObject;
        _skull = transform.GetChild(1).gameObject;
        _meshRenderer = _skeleton.GetComponent<MeshRenderer>();
        _meshRendererSkull = _skull.GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        if (!_done)
        {
            if (Physics.CheckBox(_center, _size, Quaternion.identity, (1 << 3)))
            {
                CloseDoor();
                _active = true;
            }

            else
            {
                OpenDoor();
                _active = false;
            }
        }

        if(_started && !_done)
        {
            Collider[] hitColliders = Physics.OverlapBox(_center, _size, Quaternion.identity, (1 << 3));

            foreach (Collider collider in hitColliders)
            {
                GameObject go = collider.transform.parent.gameObject;
                if (go.GetComponent<PlayerControllerE>().PlayerColor == RequiredColor)
                {
                    _done = true;
                    OpenDoor();
                    DeActivateSkeleton();
                    ActivateSkull();
                    break;
                }
            }
        }
    }

    public void ActivateSkeleton()
    {
        _skeleton.SetActive(true);
    }

    public void DeActivateSkeleton()
    {
        _skeleton.SetActive(false);
    }

    public void ActivateSkull()
    {
        _skull.SetActive(true);
    }

    public void DeActivateSkull()
    {
        _skull.SetActive(false);
    }

    public void CloseDoor()
    {
        if (!_closed)
        {
            _door.transform.eulerAngles -= new Vector3(0, 90.0f, 0);
            _closed = true;
        }
    }

    public void OpenDoor()
    {
        if (_closed)
        {
            _door.transform.eulerAngles += new Vector3(0, 90.0f, 0);
            _closed = false;
        }
    }
}
