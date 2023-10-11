using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMove : MonoBehaviourPun
{
  [SerializeField]
  private float walkSpeed, JumpPower;
  private Rigidbody myRigid;
    public Boolean canJump = true;

  void Start()
  {
    myRigid = GetComponent<Rigidbody>();
  }

  void Update()
  {
    if (photonView.IsMine)
    {
      Move();
      Jump();
    }
  }

  private void Move()
  {
    float _moveDirX = Input.GetAxisRaw("Horizontal");
    float _moveDirZ = Input.GetAxisRaw("Vertical");

    Vector3 _moveHoizontal = transform.right * _moveDirX;
    Vector3 _moveVertial = transform.forward * _moveDirZ;

    Vector3 _velocity = (_moveHoizontal + _moveVertial).normalized * walkSpeed;

    myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
  }

  private void Jump()
  {
      if (Input.GetKeyDown(KeyCode.Space) && canJump)
      {
          myRigid.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
          canJump = false;
      }
  }

    // 충돌 함수
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tetromino"))
        {
            canJump = true;
        }
    }
}
