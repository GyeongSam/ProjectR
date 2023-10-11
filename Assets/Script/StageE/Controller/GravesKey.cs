using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GravesKey : MonoBehaviourPun
{
  GameObject _linkedKey;

  void Start()
  {
    _linkedKey = transform.GetChild(0).gameObject;
  }

  private void OnEnable()
  {
    _linkedKey = transform.GetChild(0).gameObject;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Obstacle")
    {
      _linkedKey.SetActive(false);
    }
  }

  void OnTriggerExit(Collider other)
  {
    if (other.gameObject.tag == "Obstacle")
    {
      _linkedKey.SetActive(true);
    }
  }
}
