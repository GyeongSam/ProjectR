using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class PlayerControllerE : MonoBehaviourPunCallbacks
{
    public Material[] materials;
    public static GameObject LocalPlayerInstance;

    [SerializeField]
    float _speedOrigin = 3f;
    Rigidbody _rb;
    GameObject _go;

    [SerializeField]
    Define.PlayerColor _playerColor;

    Vector3 _offset = new Vector3(0.0f, 0.6f, 0.0f);

    bool _isCatched = false;

    public bool IsCatched
    {
        get { return _isCatched; }
        set { _isCatched = value; }
    }

    public Define.PlayerColor PlayerColor
    {
    get { return _playerColor; }
    set { _playerColor = value; }
    }

    bool _isJump;

    public bool IsJump
    {
    get { return _isJump; }
    set { _isJump = value; }
    }

    FloesMoveStartScript floesMoveStartScript;

    //Smooth.SmoothSyncPUN2 smpun2;

    GameObject[] _players;

    Vector3 _size;
    int _nextPlayer;

    GameObject _recallPosition;
    Material[] _materials = new Material[2];
    SkinnedMeshRenderer _meshRenderer;
    Material[] mats;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        if (photonView.IsMine)
        {
            Managers.Input.moveAction -= Move;
            Managers.Input.moveAction += Move;

            Managers.Input.jumpAction -= Jump;
            Managers.Input.jumpAction += Jump;

            Managers.Input.recallAction -= PlayerRecall;
            Managers.Input.recallAction += PlayerRecall;

            Managers.Input.recallCheckAction -= PlayerRecallCheck;
            Managers.Input.recallCheckAction += PlayerRecallCheck;

        }
        _nextPlayer = PhotonNetwork.LocalPlayer.ActorNumber == 6 ? 1 : PhotonNetwork.LocalPlayer.ActorNumber + 1;
        _size = new Vector3(0.31f, 0.362f, 0.31f);
        _speedOrigin = 3.0f;

        //smpun2 = GetComponent<Smooth.SmoothSyncPUN2>();

        floesMoveStartScript = GameObject.FindGameObjectWithTag("Floes").GetComponent<FloesMoveStartScript>();

        _recallPosition = GameObject.FindGameObjectWithTag("RecallPosition");
        _materials[0] = Resources.Load<Material>("StageE/mat1");
        _materials[1] = Resources.Load<Material>("StageE/mat2");
        _meshRenderer = _recallPosition.GetComponent<SkinnedMeshRenderer>();
    }
    void FixedUpdate()
    {
       /*if (!photonView.IsMine) return;                   
           smpun2.teleportOwnedObjectFromOwner();*/
    }

    void Move()
    {
        if (!_isCatched && photonView.IsMine)
        {
            float mv = Input.GetAxis("Vertical");
            float mh = Input.GetAxis("Horizontal");

            Vector3 heading = mv * transform.forward + mh * transform.right;

            float speed = _speedOrigin;
            
            /*if (Physics.Raycast(transform.position + _offset, heading.normalized, 0.5f))
            {
                speed = 1.0f;
                _rb.constraints |= RigidbodyConstraints.FreezePositionY;
            }

            else
                _rb.constraints &= ~(RigidbodyConstraints.FreezePositionY);*/

            float moveSpeed = Mathf.Min(heading.magnitude, 1.0f) * speed;

            _rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * heading.normalized);

            //smpun2.forceStateSendNextOnPhotonSerializeView();
        }
    }

    void Jump()
    {
        if (!_isCatched && !_isJump && photonView.IsMine)
        {
            _rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            _isJump = true;
        }
    }
    void PlayerRecall()
    {
        if (photonView.IsMine)
        {
            _recallPosition.SetActive(false);
            Vector3 center = (transform.position + transform.up * 0.362f) + Camera.main.transform.forward * 2.0f;
            if (!Physics.Raycast(transform.position + transform.up * 0.362f, Camera.main.transform.forward, 2.0f, ~(1 << 7)) && !Physics.CheckBox(center, _size))
            {
                Vector3 newPosition = transform.position + Camera.main.transform.forward * 2.0f;
                photonView.RPC("PlayerRecallRPC", RpcTarget.AllBufferedViaServer, _nextPlayer, newPosition);
            }
        }
    }
    void PlayerRecallCheck()
    {
        if (photonView.IsMine)
        {
            _recallPosition.SetActive(true);
            Vector3 center = (transform.position + transform.up * 0.362f) + Camera.main.transform.forward * 2.0f;
            _recallPosition.transform.position = transform.position + Camera.main.transform.forward * 2.0f; ;
            if (!Physics.Raycast(transform.position + transform.up * 0.362f, Camera.main.transform.forward, 2.0f, ~(1 << 7)) && !Physics.CheckBox(center, _size))
            {
                mats = _meshRenderer.materials;
                mats[0] = _materials[0];
                _meshRenderer.materials = mats;
            }

            else
            {
                mats = _meshRenderer.materials;
                mats[0] = _materials[1];
                _meshRenderer.materials = mats;
            }
        }
    }

    [PunRPC]
    void PlayerRecallRPC(int PlayerActorNoFromRPC, Vector3 position)
    {
        if (_players == null)
        {
            _players = GameObject.FindGameObjectsWithTag("Player");
        }

        foreach (GameObject player in _players)
        {
            if (player.GetComponent<PlayerControllerE>()?.PlayerColor == (Define.PlayerColor)(PlayerActorNoFromRPC - 1))
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.constraints &= ~(RigidbodyConstraints.FreezePosition);
                rb.velocity = Vector3.zero;
                break;
            }
        }

        int myActorNo = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PlayerActorNoFromRPC == myActorNo)
        {
            PlayerControllerE.LocalPlayerInstance.transform.position = position;
            PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().IsCatched = false;
        }
    }
}
