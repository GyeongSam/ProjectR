using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloesMoveStartScript : MonoBehaviour
{
    int _childCount;
    GameObject[] _floes;
    int _moveEnd;

    [SerializeField]
    GameObject _shootLaser;

    bool _moveStart = false;

    public int MoveEnd
    {
        get { return _moveEnd; }
        set { _moveEnd = value; }
    }
    public bool MoveStart
    {
        get { return _moveStart; }
        set { _moveStart = value; }
    }

    void Start()
    {
        _moveEnd = 0;
        _childCount = transform.childCount;
        _floes = new GameObject[_childCount];

        for(int i = 0; i < _childCount; i++)
            _floes[i] = transform.GetChild(i).gameObject;
    }
    IEnumerator MoveFloes()
    {
        _moveStart = true;
        _moveEnd = 0;

        for (int i = 0; i < _childCount; i++)
        {
            _floes[i].GetComponent<FloesMoveScript>().StartMoveCoroutine();
        }

        while(_moveEnd != _childCount)
        {
            yield return null;
        }
        _moveStart = false;
        _shootLaser.SetActive(true);
    }

    IEnumerator ReturnFloes(GameObject go)
    {
        _moveStart = true;
        _moveEnd = 0;

        for (int i = 0; i < _childCount; i++)
        {
            _floes[i].GetComponent<FloesMoveScript>().ReturnMoveCoroutine();
        }

        while (_moveEnd != _childCount)
        {
            yield return null;
        }

        _moveStart = false;
        StartCoroutine(RotateGiant(go));
    }

    IEnumerator RotateGiant(GameObject go)
    {
        float destX = 97.5f;
        float moveSpeed = 0.2f;
        float increase = 0.03f;
        float x = 0;

        
        while(x < destX)
        {
            x += moveSpeed * Time.deltaTime;
            go.transform.rotation = Quaternion.Euler(x, -180, 0);
            moveSpeed += increase * (x / 3);

            yield return new WaitForFixedUpdate();
        }

        go.transform.rotation = Quaternion.Euler(destX, -180, 0);
    }

    public void StartMoveFloes()
    {
        StartCoroutine(MoveFloes());
    }

    public void StartReturnFloes(GameObject go)
    {
        StartCoroutine(ReturnFloes(go));
    }
}
