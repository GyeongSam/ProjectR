using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Photon.Pun;

public class MagnetController : MonoBehaviourPun
{
  Dictionary<Define.PlayerColor, Color> _defineColor = new Dictionary<Define.PlayerColor, Color>()
    {
        { Define.PlayerColor.None, Color.white },
        { Define.PlayerColor.Red, Color.red },
        { Define.PlayerColor.Orange, new Color(203f/255f, 61f/255f, 0/255f) },
        { Define.PlayerColor.Yellow, Color.yellow },
        { Define.PlayerColor.Green, Color.green },
        { Define.PlayerColor.Blue, Color.blue },
        { Define.PlayerColor.Purple, new Color(145f/255f, 0/255f, 145f/255f) }
    };

  bool _catch;

  [SerializeField]
  Material[] _colors;

  Color _color;

  [SerializeField]
  Define.PlayerColor _playerColor;

  Coroutine _co;
  void Start()
  {
    _catch = false;
    _color = _defineColor[_playerColor];
    _co = PhotonNetwork.IsMasterClient ? StartCoroutine(ChangeColor()) : null;
  }

  void LateUpdate()
  {
    RaycastHit hit;
    if (Physics.SphereCast(transform.position, 0.8f, transform.up, out hit, 20.0f, (1 << 3) | (1 << 6)))
    {
      if (!_catch && hit.transform.gameObject.CompareTag("Player"))
      {
        GameObject player = hit.transform.gameObject;
        PlayerControllerE playerController = player.GetComponent<PlayerControllerE>();
        if (_playerColor == playerController.PlayerColor)
        {
          _catch = true;

          if (_co != null) StopCoroutine(_co);
          _co = null;
          
          if(player == PlayerControllerE.LocalPlayerInstance)
            playerController.IsCatched = true;

          Rigidbody rb = player.GetComponent<Rigidbody>();
          rb.useGravity = false;
          StartCoroutine(CharacterMove(rb));
        }
      }
    }

    else
      _catch = false;
  }


  IEnumerator ChangeColor()
  {
    if (!PhotonNetwork.IsMasterClient)
      yield return null;

    while (true)
    {
      _playerColor = (Define.PlayerColor)(((int)_playerColor + 2) % 6);

      photonView.RPC("MaterialChangeRPC", RpcTarget.AllViaServer, _playerColor);

      yield return new WaitForSeconds(1.5f);
    }
  }

  IEnumerator CharacterMove(Rigidbody rb)
  {
    float moveSpeed = 20.0f;
    while (true)
    {
      float distance = Vector3.Distance(transform.position, rb.position + Vector3.up / 2);

      if (distance < 1.0f) moveSpeed = 8.0f;
      rb.MovePosition(rb.position + (transform.position - (rb.position + Vector3.up / 2)).normalized * moveSpeed * Time.deltaTime);
      yield return new WaitForFixedUpdate();
      if (Vector3.Distance(transform.position, rb.position + Vector3.up / 2) < 0.4f)
      {
        rb.constraints |= RigidbodyConstraints.FreezePosition;
        rb.useGravity = true;
        break;
      }
    }
  }

  void MaterialChange(MeshRenderer meshRenderer)
  {
    Material[] mat = meshRenderer.materials;
    mat[0] = _colors[(int)_playerColor];
    meshRenderer.materials = mat;
  }

  [PunRPC]
  public void MaterialChangeRPC(Define.PlayerColor playerColor)
  {
    _playerColor = playerColor;

    for (int i = 0; i < transform.childCount; i++)
    {
      MaterialChange(transform.GetChild(i).GetComponent<MeshRenderer>());
    }
  }
}
