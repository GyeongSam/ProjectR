using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerButtonController : MonoBehaviour
{
    Coroutine _timer;
    Coroutine _check;
    public Coroutine Timer
    {
        get { return _timer; }
        set { _timer = value; }
    }
    public Coroutine Check
    {
        get { return _check; }
        set { _check = value; }
    }

    int _childCount;

    PushButtonController[] _pushButtonScripts;
    GameObject[] _pushButtons;

    [SerializeField]
    GameObject _key;
    void Start()
    {
        _childCount = transform.childCount;

        _pushButtons = new GameObject[_childCount];
        _pushButtonScripts = new PushButtonController[_childCount];

        for (int i = 0; i < _childCount; i++)
        {
            _pushButtons[i] = transform.GetChild(i).GetChild(0).gameObject;
            _pushButtonScripts[i] = _pushButtons[i].GetComponent<PushButtonController>();
        }
    }

    public IEnumerator TimerCoroutine(float time, string soundPath)
    {
        yield return new WaitForSeconds(time);
        TimerEnd();
    }

    public IEnumerator CheckCoroutine()
    {
        while (true)
        {
            int i = 0;
            for (; i < _childCount; i++)
            {
                if (!_pushButtonScripts[i].On)
                    break;
            }

            if (i == _childCount)
            {
                StopCoroutine(_timer);
                _timer = null;

                StartCoroutine(TimerCoroutine(1.0f, null));
                for (int j = 0; j < _childCount; j++)
                {
                    _pushButtonScripts[j].Done = true;
                }
                _key.SetActive(true);
                break;
            }

            yield return null;
        }
    }

    void TimerEnd()
    {
        Debug.Log("³¡");
        for (int i = 0; i < _childCount; i++)
        {
            if (_pushButtonScripts[i].On) _pushButtonScripts[i].Open();
            _pushButtonScripts[i].On = false;
        }

        if(_timer != null) StopCoroutine(_timer);
        if(_check != null) StopCoroutine(_check);
        _timer = null;
        _check = null;
    }

    public void StartTimer()
    {
        Debug.Log("½ÃÀÛ");
        _timer = StartCoroutine(TimerCoroutine(5.0f, null));
        _check = StartCoroutine(CheckCoroutine());
    }
}

