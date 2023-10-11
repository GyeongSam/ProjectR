using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class JailRoomController : MonoBehaviourPun
{
  int _childCount;
  RoomController[] _roomScripts;
  GameObject[] _rooms;

  bool _isStarted = false;
  bool _success = false;

  [SerializeField]
  Material[] _color;

  [SerializeField]
  GameObject _key;
  void Start()
  {
    _childCount = transform.childCount;

    _rooms = new GameObject[_childCount];
    _roomScripts = new RoomController[_childCount];

    for (int i = 0; i < _childCount; i++)
    {
      _rooms[i] = transform.GetChild(i).gameObject;
      _roomScripts[i] = _rooms[i].GetComponent<RoomController>();
      _roomScripts[i].RequiredColor = (Define.PlayerColor)i;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (!_isStarted)
    {
      int i = 0;
      for (; i < _childCount; i++)
      {
        if (!_roomScripts[i].Active) break;
      }

      if (i == _childCount && PhotonNetwork.IsMasterClient)
      {
        photonView.RPC("StartPuzzleRPC", RpcTarget.AllBufferedViaServer, Random.Range(1, 6));
      }
    }
  }

  void StartPuzzle(int k)
  {
    _isStarted = true;

    for (int i = 0; i < _childCount; i++)
    {
      int idx = ((int)_roomScripts[i].RequiredColor + k) % 6;
      _roomScripts[i].RequiredColor = (Define.PlayerColor)idx;

      Material[] mats = _roomScripts[i].MeshRenderer.materials;
      mats[0] = _color[idx];
      _roomScripts[i].MeshRenderer.materials = mats;

      mats = _roomScripts[i].MeshRendererSkull.materials;
      mats[0] = _color[idx];
      _roomScripts[i].MeshRendererSkull.materials = mats;

      _roomScripts[i].ActivateSkeleton();
      _roomScripts[i].Started = true;
    }

    StartCoroutine(Start_Puzzle());
  }

  IEnumerator Start_Puzzle()
  {
    while (true)
    {
      int i = 0;
      int count = 0;
      for (; i < _childCount; i++)
      {
        if (_roomScripts[i].Done)
        {
          count++;
          continue;
        }
        if (!_roomScripts[i].Active) break;
      }

      if (i != _childCount)
      {
        End_Puzzle();
        break;
      }

      if (count == _childCount)
      {
        _success = true;
        End_Puzzle();
        break;
      }

      yield return new WaitForFixedUpdate();
    }
  }

  void End_Puzzle()
  {
    if (_success)
    {
      _key.SetActive(true);
      _isStarted = true;
    }

    else
    {
      for (int i = 0; i < _childCount; i++)
      {
        _roomScripts[i].Done = false;
        _roomScripts[i].Started = false;
        _roomScripts[i].DeActivateSkeleton();
        _roomScripts[i].DeActivateSkull();
      }
      _isStarted = false;
    }
  }

  [PunRPC]
  void StartPuzzleRPC(int k)
  {
    StartPuzzle(k);
  }

  [PunRPC]
  void EndPuzzleRPC()
  {
    End_Puzzle();
  }
}
