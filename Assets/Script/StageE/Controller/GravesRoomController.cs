using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GravesRoomController : MonoBehaviour, IInstanceManager
{
  [SerializeField]
  GameObject _graves;

  [SerializeField]
  GameObject _keyGreen;

  [SerializeField]
  GameObject _door;
  int _randidx;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _randidx = Random.Range(0, _graves.transform.childCount);

            //_keyGreen.transform.position = _graves.transform.GetChild(_randidx).position + _graves.transform.up * 0.5f;
            Vector3 keyPosition = _graves.transform.GetChild(_randidx).position + _graves.transform.up * 0.5f;

            IInstanceManager.Instantiate(IInstanceManager.StagePath.StageE, "Key(green)", keyPosition, Quaternion.identity);
        }
    }
}
