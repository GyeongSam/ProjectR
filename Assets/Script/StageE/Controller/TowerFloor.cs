using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TowerFloor : MonoBehaviour
{ 
    Vector3 _size = new Vector3(1.655f, 0.2f, 1.655f);
    Vector3 _originTransform;
    Vector3 _destination;
    Vector3 _offSet = new Vector3(0f, 0.35f, 0f);
    bool _startMove = false;
    int _count;

    SpriteRenderer[] _numbersSprite;
    void Start()
    {
        _originTransform = transform.position;
        _destination = _originTransform + new Vector3(0, 9.5f, 0);

        _numbersSprite = new SpriteRenderer[transform.GetChild(0).GetChild(2).childCount];
        for (int i = 0; i < _numbersSprite.Length; i++)
        {
            _numbersSprite[i] = transform.GetChild(0).GetChild(2).GetChild(i).GetComponent<SpriteRenderer>();
        }
    }
    void Update()
    {
        Collider[] players = CheckPlayer();
        _count = players.Length;

        if (!_startMove)
        {
            SetNumber();
        }

        if (!_startMove && _count == 6)
        {
            _startMove = true;

            PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().IsCatched = true;
            StartCoroutine(StartMove(players));     
        }
    }

    Collider[] CheckPlayer()
    {
        return Physics.OverlapBox(transform.position + _offSet, _size, Quaternion.identity, (1 << 3));
    }

    void SetNumber()
    {
        switch (_count)
        {
            case 0:
                _numbersSprite[0].color = Color.red;
                _numbersSprite[1].color = Color.red;
                _numbersSprite[2].color = Color.clear;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.red;
                _numbersSprite[5].color = Color.red;
                _numbersSprite[6].color = Color.red;
                break;
            case 1:
                _numbersSprite[0].color = Color.red;
                _numbersSprite[1].color = Color.clear;
                _numbersSprite[2].color = Color.clear;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.red;
                _numbersSprite[5].color = Color.red;
                _numbersSprite[6].color = Color.red;
                break;
            case 2:
                _numbersSprite[0].color = Color.red;
                _numbersSprite[1].color = Color.clear;
                _numbersSprite[2].color = Color.red;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.clear;
                _numbersSprite[5].color = Color.red;
                _numbersSprite[6].color = Color.clear;
                break;
            case 3:
                _numbersSprite[0].color = Color.clear;
                _numbersSprite[1].color = Color.clear;
                _numbersSprite[2].color = Color.red;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.red;
                _numbersSprite[5].color = Color.red;
                _numbersSprite[6].color = Color.red;
                break;
            case 4:
                _numbersSprite[0].color = Color.clear;
                _numbersSprite[1].color = Color.red;
                _numbersSprite[2].color = Color.red;
                _numbersSprite[3].color = Color.clear;
                _numbersSprite[4].color = Color.red;
                _numbersSprite[5].color = Color.red;
                _numbersSprite[6].color = Color.red;
                break;
            case 5:
                _numbersSprite[0].color = Color.clear;
                _numbersSprite[1].color = Color.clear;
                _numbersSprite[2].color = Color.red;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.clear;
                _numbersSprite[5].color = Color.clear;
                _numbersSprite[6].color = Color.clear;
                break;
            default:
                _numbersSprite[0].color = Color.red;
                _numbersSprite[1].color = Color.red;
                _numbersSprite[2].color = Color.red;
                _numbersSprite[3].color = Color.red;
                _numbersSprite[4].color = Color.red;
                _numbersSprite[5].color = Color.clear;
                _numbersSprite[6].color = Color.red;
                break;
        }
    }

    IEnumerator StartMove(Collider[] players)
    {
        while (true)
        {
            if (transform.position.y > _destination.y)
            {
                transform.position = _destination + new Vector3(0, 0.01f, 0);
                break;
            }

            else if (transform.position.y < _destination.y)
            {
                foreach (Collider c in players)
                {
                    Rigidbody rb = c.transform.parent.GetComponent<Rigidbody>();
                    rb.MovePosition(rb.position + 0.6f * Time.deltaTime * transform.up);
                }
                transform.Translate(0.6f * Time.deltaTime * Vector3.up);
            }

            yield return new WaitForFixedUpdate();
        }
        PlayerControllerE.LocalPlayerInstance.GetComponent<PlayerControllerE>().IsCatched = false;
    }
}
