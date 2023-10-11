using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VillagePlayerController : MonoBehaviourPun
{
  [SerializeField]
  private float walkSpeed = 20;
  private Rigidbody _rid;

  [SerializeField]
  /*Vector3 RespawnPos = new Vector3(107.88f, -119.5f, 346.34f);*/

  public float[] RespawnXPos = new float[] { 107.88f, 1005.808f, 174.2501f };
  public float[] RespawnYPos = new float[] { -119.5f, -156.9288f, -147.6431f };
  public float[] RespawnZPos = new float[] { 346.34f, 542.2339f, -468.1222f };

  Smooth.SmoothSyncPUN2 smpun2;

  void Start()
  {
    _rid = GetComponent<Rigidbody>();
    smpun2 = GetComponent<Smooth.SmoothSyncPUN2>();
  }

  void Update()
  {

    Move();
    Respawn();
  }

  void Move()
  {
    float _moveDirX = Input.GetAxisRaw("Horizontal");
    float _moveDirZ = Input.GetAxisRaw("Vertical");

    Vector3 _moveHorizontal = transform.right * _moveDirX;
    Vector3 _moveVertical = transform.forward * _moveDirZ;
    /*Vector3 heading = _moveDirZ * transform.forward + _moveDirX * transform.right;*/

    Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

    _rid.MovePosition(transform.position + _velocity * Time.deltaTime);
  }

  void Respawn()
  {
    /*if (_isActive)
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = RespawnPos;

    }*/
    if (Input.GetKeyDown(KeyCode.R))
    {
      int rand = UnityEngine.Random.Range(0, 3);

      Vector3 RespawnPos = new Vector3(RespawnXPos[rand], RespawnYPos[rand], RespawnZPos[rand]);
      GameObject.FindGameObjectWithTag("Player").transform.position = RespawnPos;
    }
  }
}
