using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GroundCheckController : MonoBehaviour
{
  //PlayerControllerEForTesting _player;
  PlayerControllerE _player;

  void Start()
    {
    //_player = transform.parent.gameObject.GetComponent<PlayerControllerEForTesting>();
    _player = transform.parent.gameObject.GetComponent<PlayerControllerE>();
  }

  void OnTriggerEnter(Collider other)
    {
        _player.IsJump = false;
    }
}
