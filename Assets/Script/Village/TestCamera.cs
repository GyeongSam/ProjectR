using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public GameObject player; // 바라볼 플레이어 오브젝트입니다.
    public float xmove = 0;  // X축 누적 이동량
    public float ymove = 0;  // Y축 누적 이동량
    public float distance = 3;

    public float SmoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    private int toggleView = 3; // 1=1인칭, 3=3인칭

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
        //  마우스 우클릭 중에만 카메라 무빙 적용
        if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.
        }
        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // 이동량에 따라 카메라의 바라보는 방향을 조정합니다.

        if (Input.GetMouseButtonDown(2))
            toggleView = 4 - toggleView;

        if (toggleView == 1)
        {
            Vector3 reverseDistance = new Vector3(0.0f, 0.4f, 0.2f); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
            transform.position = player.transform.position + transform.rotation * reverseDistance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.
        }
        else if (toggleView == 3)
        {
            Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
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