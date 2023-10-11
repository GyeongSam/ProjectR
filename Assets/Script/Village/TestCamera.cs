using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public GameObject player; // �ٶ� �÷��̾� ������Ʈ�Դϴ�.
    public float xmove = 0;  // X�� ���� �̵���
    public float ymove = 0;  // Y�� ���� �̵���
    public float distance = 3;

    public float SmoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    private int toggleView = 3; // 1=1��Ī, 3=3��Ī

    [SerializeField]
    GameObject[] _players;
    GameObject _player;
    int _index = 0;

    GameObject[] _interactiveObjects;
    public int PlayerIndex { get { return _index; } }
    public GameObject Player { get { return _player; } }
    float _rotationX;

    Vector3 _offset;
    Vector3 _size;
    Rigidbody _playerRb;

    void Start()
    {

        /*_players = GameObject.FindGameObjectsWithTag("Player");*/
        _player = _players[_index];

        _playerRb = _player.GetComponent<Rigidbody>();

        _player.GetComponent<PlayerControllerVillage>().IsActive = true;
        _player.GetComponent<PlayerControllerVillage>().enabled = true;

        //_interactiveObjects = GameObject.FindGameObjectsWithTag("InteractionObject");

        _offset = new Vector3(0.0f, 1f, 2f);
        _size = new Vector3(0.3f, 0.5f, 0.3f);
        ResetCam();
    }

    // Update is called once per frame
    void Update()
    {
        //  ���콺 ��Ŭ�� �߿��� ī�޶� ���� ����
        if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // ���콺�� �¿� �̵����� xmove �� �����մϴ�.
            ymove -= Input.GetAxis("Mouse Y"); // ���콺�� ���� �̵����� ymove �� �����մϴ�.
        }
        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // �̵����� ���� ī�޶��� �ٶ󺸴� ������ �����մϴ�.

        if (Input.GetMouseButtonDown(2))
            toggleView = 4 - toggleView;

        if (toggleView == 1)
        {
            Vector3 reverseDistance = new Vector3(0.0f, 0.4f, 0.2f); // ī�޶� �ٶ󺸴� �չ����� Z ���Դϴ�. �̵����� ���� Z ������� ���͸� ���մϴ�.
            transform.position = player.transform.position + transform.rotation * reverseDistance; // �÷��̾��� ��ġ���� ī�޶� �ٶ󺸴� ���⿡ ���Ͱ��� ������ ��� ��ǥ�� �����մϴ�.
        }
        else if (toggleView == 3)
        {
            Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // ī�޶� �ٶ󺸴� �չ����� Z ���Դϴ�. �̵����� ���� Z ������� ���͸� ���մϴ�.
            transform.position = Vector3.SmoothDamp(
                transform.position,
                player.transform.position - transform.rotation * reverseDistance,
                ref velocity,
                SmoothTime);
        }
    }

    public void ResetCam()
    {
        transform.position = _player.transform.position + _offset;
        transform.rotation = Quaternion.LookRotation(_player.transform.forward);
        _rotationX = transform.eulerAngles.x;
    }
}