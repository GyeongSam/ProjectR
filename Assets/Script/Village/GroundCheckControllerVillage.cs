using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckControllerVillage : MonoBehaviour
{
  PlayerControllerVillage _player;

  void Start()
  {
    _player = transform.parent.gameObject.GetComponent<PlayerControllerVillage>();
  }

  void OnTriggerEnter(Collider other)
  {
    if (_player != null)
      _player.IsJump = false;
  }
}
