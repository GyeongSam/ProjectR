using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class KeyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Define.PlayerColor _keyColor;

    public Define.PlayerColor KeyColor
    {
        get { return _keyColor; }
    }

    GameObject _player;

    Camera _mainCamera;
    CameraController _mainCameraScript;
    GameObject[] _players;

    bool _timeEnd;

    float xRotate, yRotate, zRotate;

    void Start()
    {
        xRotate = transform.eulerAngles.x;
        yRotate = transform.eulerAngles.y;
        zRotate = transform.eulerAngles.z;  
    }

    void Update()
    {
        yRotate += 0.3f;
        transform.eulerAngles = new Vector3(xRotate, yRotate, zRotate);
    }

    private void OnTriggerEnter(Collider other)
    {
        _player = other.transform.gameObject;
        if (_player.CompareTag("Player"))
        {
            _player = _player.transform.parent.gameObject;

            Define.PlayerColor nplayerColor = _player.GetComponent<PlayerControllerE>().PlayerColor;

            if (nplayerColor == _keyColor && PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().PlayerColor == nplayerColor)
            {              
                photonView.RPC("OpenDoorRPC", RpcTarget.AllBufferedViaServer, (int)_keyColor);
                //StartCoroutine(OpenDoor(doors));
            }
        }
    }

    [PunRPC]
    void OpenDoorRPC(int keyColor)
    {
        GameObject[] doors;

        switch (keyColor)
        {
            case 2:
                doors = new GameObject[2] { Define.doors.transform.GetChild(0).gameObject, Define.doors.transform.GetChild(1).gameObject };
                break;
            case 3:
                doors = new GameObject[1] { Define.doors.transform.GetChild(2).gameObject };
                break;
            case 4:
                doors = new GameObject[1] { Define.doors.transform.GetChild(3).gameObject };
                break;
            case 5:
                doors = new GameObject[1] { Define.doors.transform.GetChild(4).gameObject };
                break;
            case 0:
                doors = new GameObject[2] { Define.doors.transform.GetChild(5).gameObject, Define.doors.transform.GetChild(6).gameObject };
                break;
            default:
                doors = new GameObject[2] { Define.doors.transform.GetChild(7).GetChild(1).gameObject, Define.doors.transform.GetChild(7).GetChild(2).gameObject };
                break;
        }

        if(_players == null)
        {
            _players = GameObject.FindGameObjectsWithTag("Player");
        }

        foreach (GameObject p in _players) 
        {
            Rigidbody rb = p.GetComponent<Rigidbody>();
            if(rb != null) rb.constraints |= RigidbodyConstraints.FreezePosition;
        }

        StartCoroutine(OpenDoor(doors));
    }

    IEnumerator OpenDoor(GameObject[] doors)
    {
        _mainCamera = Camera.main;
        _mainCameraScript = _mainCamera.GetComponent<CameraController>();
        
        foreach (GameObject door in doors)
        {
           yield return StartCoroutine(MoveDoor(door));
        }

        _mainCameraScript.enabled = true;

        _mainCamera.GetComponent<CameraController>().ResetCam();
        Managers.Input.IsActive = true;

        if (_players == null)
        {
            _players = GameObject.FindGameObjectsWithTag("Player");
        }

        foreach (GameObject p in _players)
        {
            if (p.GetComponent<PlayerControllerE>() != null)
            {
                if (!p.GetComponent<PlayerControllerE>().IsCatched)
                {
                    Rigidbody rb = p.GetComponent<Rigidbody>();
                    if (rb != null) rb.constraints &= ~(RigidbodyConstraints.FreezePosition);
                }
            }
        }

        Destroy(gameObject);
    }

    IEnumerator MoveDoor(GameObject door)
    {
        Managers.Input.IsActive = false;
        _mainCameraScript.enabled = false;
        
        _mainCamera.transform.position = door.transform.position + door.transform.forward * 3.0f;
        _mainCamera.transform.position += door.transform.up * 2.0f;
        _mainCamera.transform.LookAt(door.transform.position);

        StartCoroutine(TimeCheck());

        SoundPack soundPack = GameObject.FindGameObjectWithTag("Sound").GetComponent<SoundPack>();
        soundPack.Play(1, 0.8f);

        DoorController doorController = door.GetComponent<DoorController>();

        Define.MoveDirection md = doorController.MoveDirection;
        Vector3 moveDirection = md switch
        {
            Define.MoveDirection.Up => Vector3.up,
            Define.MoveDirection.Down => Vector3.down,
            Define.MoveDirection.Left => Vector3.left,
            Define.MoveDirection.Right => Vector3.right,
            Define.MoveDirection.Forward => Vector3.forward,
            Define.MoveDirection.Back => Vector3.back,
            _ => Vector3.zero,
        };

        _timeEnd = false;
        float moveSpeed = 0.5f;
        while (!_timeEnd)
        {
            door.transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
            yield return null;
        }

        door.SetActive(false);
    }

    IEnumerator TimeCheck()
    {
        yield return new WaitForSeconds(3.0f);
        _timeEnd = true;
    }
}
