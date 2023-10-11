using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerControllerVillage : MonoBehaviourPun
{
  public Animator ani;
  public Transform UI;
  public VillageManager vm;
  [SerializeField]
  bool _getKey;

  public bool GetKey {
    get { return _getKey; }
    set { _getKey = value; }
  }

  [SerializeField]
  float _speedOrigin = 20f;
  Rigidbody _rb;
  GameObject _go;

  //InteractiveObjectController _ioc;

  Define.Condition _condition = Define.Condition.None;

  [SerializeField]
  Define.PlayerColor _playerColor;

  GameObject _stuff;

  Animator _anim;

  int framecount=0;

  public GameObject Stuff {
    get { return _stuff; }
    set { _stuff = value; }
  }

  bool _isActive = true;

  public bool IsActive {
    get { return _isActive; }
    set { _isActive = value; }
  }

  public Define.Condition Condition {
    get { return _condition; }
    set { _condition = value; }
  }

  public Define.PlayerColor PlayerColor {
    get { return _playerColor; }
    set { _playerColor = value; }
  }

  [SerializeField]
  bool _isJump;

  public bool IsJump {
    get { return _isJump; }
    set { _isJump = value; }
  }

  [SerializeField]
  GameObject _onhead = null;
  public GameObject OnHead {
    get { return _onhead; }
    set { _onhead = value; }
  }

  [SerializeField]
  /*Vector3 RespawnPos = new Vector3(107.88f, -119.5f, 346.34f);*/

  public float[] RespawnXPos = new float[] { 107.88f, 1005.808f, 174.2501f };
  public float[] RespawnYPos = new float[] { -119.5f, -156.9288f, -147.6431f };
  public float[] RespawnZPos = new float[] { 346.34f, 542.2339f, -468.1222f };


  Smooth.SmoothSyncPUN2 smpun2;
  Camera mainCam;
  void Start()
  {
    if (!photonView.IsMine)
      return;
    ani = GetComponent<Animator>();
    vm = GameObject.Find("VillageManager").GetComponent<VillageManager>();
    UI = GameObject.Find("UI").GetComponent<Transform>();

    ManagersVillage.Input.moveAction -= Move;
    ManagersVillage.Input.moveAction += Move;

    ManagersVillage.Input.jumpAction -= Jump;
    ManagersVillage.Input.jumpAction += Jump;

    _rb = GetComponent<Rigidbody>();
    //_anim = GetComponentInChildren<Animator>();

    mainCam = Camera.main;

    smpun2 = GetComponent<Smooth.SmoothSyncPUN2>();
  }

  Vector3 _offset = new Vector3(0.0f, 0.6f, 0.0f);
  private void Update()
  {
    if (!photonView.IsMine || UI.GetChild(2).gameObject.activeSelf || UI.GetChild(3).gameObject.activeSelf || UI.GetChild(1).gameObject.activeSelf) return;

    if (Input.GetKeyDown(KeyCode.F))
    {
      ani.SetTrigger("F");
    }
    else if (Input.GetKeyDown(KeyCode.G))
    {
      ani.SetTrigger("G");
    }
    else if (Input.GetKeyDown(KeyCode.E))
    {
      ani.SetTrigger("E");
    }
    else if (Input.GetKeyDown(KeyCode.M))
    {
      ani.SetTrigger("M");
    }


    if (Input.GetMouseButtonDown(0))
    {
      Debug.Log("click");
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Physics.Raycast(ray, out hit);
      if (hit.collider.CompareTag("Ranking"))
      {
        UI.GetChild(0).gameObject.SetActive(false);
        UI.GetChild(2).gameObject.GetComponent<Ranking>().gameName=hit.collider.gameObject.name;
        UI.GetChild(2).gameObject.SetActive(true);
        UI.GetChild(1).gameObject.SetActive(false);
        UI.GetChild(3).gameObject.SetActive(false);
      }
      else if (hit.collider.CompareTag("Party"))
      {
        UI.GetChild(0).gameObject.SetActive(false);
        UI.GetChild(3).gameObject.GetComponent<PartySystem>().gameType = hit.collider.gameObject.name;
        UI.GetChild(3).gameObject.SetActive(true);
        UI.GetChild(1).gameObject.SetActive(false);
        UI.GetChild(2).gameObject.SetActive(false);
      }
      else if (hit.collider.CompareTag("MyProfile"))
      {
        UI.GetChild(0).gameObject.SetActive(false);
        UI.GetChild(1).gameObject.SetActive(true);
        UI.GetChild(3).gameObject.SetActive(false);
        UI.GetChild(2).gameObject.SetActive(false);
      }
    }
  }
  void FixedUpdate()
  {
    //if (Physics.Raycast(transform.position + _offset, transform.forward, out RaycastHit hit, 1.0f, (1 << 8)))
    //{
    //    _go = hit.collider.gameObject;

    //    _ioc = _go.GetComponent<InteractiveObjectController>();

    //    _ioc.IsActive = true;

    //    _ioc.Condition = _condition;

    //    _ioc.Color = _playerColor;

    //}

    //else if (_go != null)
    //{
    //    _ioc.IsActive = false;
    //    _ioc.Condition = Define.Condition.None;
    //    _go = null;
    //}
    /*   Rotation();*/

    if (!photonView.IsMine) return;

    if (++framecount>3) ani.SetInteger("EggState", 0);

    if(PhotonNetwork.InRoom)
      smpun2.teleportOwnedObjectFromOwner();

    Respawn();
  }

  void Move()
  {
    if (transform == null)
      return;

    if (_isActive)
    {
      framecount = 0;
      ani.SetInteger("EggState", 1);
      float mv = Input.GetAxis("Vertical");
      float mh = Input.GetAxis("Horizontal");

      Vector3 heading = mv * transform.forward + mh * transform.right;

      float speed = _speedOrigin;
      if (Physics.Raycast(transform.position + _offset, heading.normalized, 0.7f))
      {
        speed = 2.0f;
        _rb.constraints |= RigidbodyConstraints.FreezePositionY;
      }

      else
        _rb.constraints &= ~(RigidbodyConstraints.FreezePositionY);

      float moveSpeed = Mathf.Min(heading.magnitude, 1.0f) * speed;

      _rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * heading.normalized);
    }
  }

  void Jump()
  {
    if (!_isJump && _isActive)
    {
      _rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
      /*_anim.SetBool("isJump", true);
      _anim.SetTrigger("doJump");*/
      _isJump = true;
    }
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
  public void ChangeEgg(int n)
  {
    photonView.RPC("ChangeEggRPC", RpcTarget.AllViaServer, n);
  }

  [PunRPC]
  public void ChangeEggRPC(int n)
  {
    GameObject temp = Resources.Load($"Characters/Egg{n}") as GameObject;
    GameObject temp2 =GameObject.Instantiate(temp, transform.position, Quaternion.identity);
    Transform temp3 = temp2.transform.GetChild(0).transform;

    transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = temp3.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
    transform.GetChild(0).gameObject.GetComponent<Renderer>().material = temp3.gameObject.GetComponent<Renderer>().material;
  }

  /*    void Rotation() 
      {
          float xMove = 0;
          float yMove = 0;

          if (Input.GetMouseButton(1))
          {
              xMove += Input.GetAxis("Mouse X");
              yMove -= Input.GetAxis("Mouse Y");
          }
          transform.rotation = Quaternion.Euler(yMove, xMove, 0);

      }*/
}
