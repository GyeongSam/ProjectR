using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControllerVillage : MonoBehaviour
{
  [SerializeField]
  GameObject _player;
  int _index = 0;

  GameObject[] _interactiveObjects;
  public int PlayerIndex { get { return _index; } }
  public GameObject Player { get { return _player; } }

  [SerializeField]
  float _turnSpeedY = 15.0f;
  [SerializeField]
  float _turnSpeedX = 2.0f;

  float _rotationX;

  Vector3 _offset;
  Vector3 _size;
  Rigidbody _playerRb;

  void Start()
  {
    ManagersVillage.Input.camAction -= CamRotation;
    ManagersVillage.Input.camAction += CamRotation;
  }

  void LateUpdate()
  {
    //transform.position = _player.transform.position + _offset;

    if (_player != null)
    {
      float distance = 5;
      Vector3 reserveDistance = new Vector3(0.0f, -distance, distance + 4);

      transform.position = _player.transform.position - transform.rotation * reserveDistance;
    }
  }

  void CamRotation()
  {
    if (_playerRb == null)
    {
      GameObject[] P= GameObject.FindGameObjectsWithTag("Player");
      foreach (GameObject p in P)
      {
        if (p.GetComponent<PlayerControllerVillage>().photonView.IsMine)
        {
          _playerRb = p.GetComponent<Rigidbody>();
          break;
        }
      }
      return;
    }
    _playerRb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
    float yRotate = (Input.GetAxis("Mouse X") * _turnSpeedY) + transform.eulerAngles.y;
    _rotationX -= (Input.GetAxis("Mouse Y") * _turnSpeedX);

    _rotationX = Mathf.Clamp(_rotationX, -60, 30);

    transform.eulerAngles = new Vector3(_rotationX, yRotate, 0);

    _player.transform.eulerAngles = new Vector3(0, yRotate, 0);
    _playerRb.constraints |= RigidbodyConstraints.FreezeRotationY;
  }

  public void SetupCam(GameObject player)
  {
    _player = player;
    _playerRb = _player.GetComponent<Rigidbody>();

    _player.GetComponent<PlayerControllerVillage>().IsActive = true;
    _player.GetComponent<PlayerControllerVillage>().enabled = true;

    _offset = new Vector3(-5f, 3f, 0.0f);
    _size = new Vector3(0.3f, 0.5f, 0.3f);
    //ResetCam();
  }

  public void ResetCam()
  {
    transform.position = _player.transform.position + _offset;
    transform.rotation = Quaternion.LookRotation(_player.transform.forward);
    _rotationX = transform.eulerAngles.x;
  }
}
