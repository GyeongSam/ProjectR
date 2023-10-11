using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Photon.Pun;

public class LaserTarget : MonoBehaviourPun
{
  bool isClearSignalReceived = false;
  int _count = 0;
  Light _light;

  [SerializeField]
  GameObject _giant;

  [SerializeField]
  GameObject _giantNoHead;

  [SerializeField]
  GameObject _laserPoint;

  public int Count
  {
    get { return _count; }
    set { _count = value; }
  }
  void Start()
  {
      _light = transform.GetChild(0).gameObject.GetComponent<Light>();

      GameObject skullHead = transform.parent.parent.gameObject;
  }

  IEnumerator CheckCount()
  {
    while (_count < 240)
    {
      _light.intensity = _count * 0.034f;
      _light.range = _count * 0.034f;
      yield return null;
    }

    photonView.RPC("SignalClearRPC", RpcTarget.AllBufferedViaServer, null);
  }

  public void StartCheck()
  {
      StartCoroutine(CheckCount());
  }

  [PunRPC]
  void SignalClearRPC()
    {
    if (!isClearSignalReceived)
        {
      Debug.LogError("Clear RPC Signaled");

      isClearSignalReceived = true;

        _giant.SetActive(false);
        _laserPoint.SetActive(false);
        _giantNoHead.SetActive(true);

        GameObject skullHead = transform.parent.parent.gameObject;

        Rigidbody skullRb = skullHead.GetComponent<Rigidbody>();
        skullRb.constraints &= ~(RigidbodyConstraints.FreezeAll);

        skullRb.AddForce((skullHead.transform.up - skullHead.transform.forward) * 40.0f, ForceMode.Impulse);
        skullRb.AddTorque(-skullHead.transform.right * 50.0f, ForceMode.Impulse);

        skullHead.transform.parent.GetChild(0).GetComponent<FloesMoveStartScript>().StartReturnFloes(_giantNoHead);
    }
  }
}
