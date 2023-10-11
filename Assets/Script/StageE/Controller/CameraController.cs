using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraController : MonoBehaviourPunCallbacks
{
    GameObject[] _players;
    public static GameObject _player;
    int _index = 0;

    public int PlayerIndex { get { return _index; } }
    public GameObject Player { get { return _player; } }

    [SerializeField]
    float _turnSpeedY = 3.0f;
    [SerializeField]
    float _turnSpeedX = 2.0f;

    float _rotationX;

    Vector3 _offset;
    Vector3 _size;
    Rigidbody _playerRb;

    public void CamSet()
    {
        Managers.Input.camAction -= CamRotation;
        Managers.Input.camAction += CamRotation;

        _playerRb = _player.GetComponent<Rigidbody>();

        _offset = new Vector3(0.0f, 0.6f, 0.0f);
        ResetCam();
    }

    void LateUpdate()
    {
        transform.position = _player.transform.position + _offset;
    }

    void CamRotation()
    {
        /*float yRotate = (Input.GetAxis("Mouse X") * _turnSpeedY) + transform.eulerAngles.y;
        _rotationX -= (Input.GetAxis("Mouse Y") * _turnSpeedX);

        _rotationX = Mathf.Clamp(_rotationX, -60, 30);

        transform.eulerAngles = new Vector3(_rotationX, yRotate, 0);

        _player.transform.eulerAngles = new Vector3(0, yRotate, 0);*/

        _playerRb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
        float yRotate = (Input.GetAxis("Mouse X") * _turnSpeedY) + transform.eulerAngles.y;
        _rotationX -= (Input.GetAxis("Mouse Y") * _turnSpeedX);

        _rotationX = Mathf.Clamp(_rotationX, -60, 30);

        transform.eulerAngles = new Vector3(_rotationX, yRotate, 0);

        _player.transform.eulerAngles = new Vector3(0, yRotate, 0);
        _playerRb.constraints |= RigidbodyConstraints.FreezeRotationY;

    }

    public void ResetCam()
    {
        transform.position = _player.transform.position + _offset;
        transform.rotation = Quaternion.LookRotation(_player.transform.forward);
        _rotationX = transform.eulerAngles.x;
    }
}
