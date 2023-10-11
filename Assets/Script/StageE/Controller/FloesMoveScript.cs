using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloesMoveScript : MonoBehaviour
{
  [SerializeField]
  Vector3 _moveDestination;

  Vector3 _originPosition;

  FloesMoveStartScript _script;

  Vector3 _size = new(1.1242783725f, 0.5f, 1.09711106f);
  Vector3 _dir;
  void Start()
  {
    _originPosition = transform.position;
    _script = transform.parent.GetComponent<FloesMoveStartScript>();
    _dir = (_moveDestination - transform.position).normalized;
  }

  IEnumerator StartMove()
  {
    while (transform.position.y < _moveDestination.y)
    {
      Collider[] players = Physics.OverlapBox(transform.position + new Vector3(0, 0.5369839f, 0), _size, Quaternion.identity, (1 << 3));

      foreach (Collider player in players)
      {
        Rigidbody rb = player.transform.parent.GetComponent<Rigidbody>();
        rb.MovePosition(player.transform.parent.position + 1.2f * Time.deltaTime * _dir);
      }

      transform.Translate(_dir * 1.2f * Time.deltaTime);

      yield return new WaitForFixedUpdate();
    }

    transform.position = _moveDestination;

    _script.MoveEnd += 1;
  }

    IEnumerator ReturnMove()
    {
        while (transform.position.y > _originPosition.y)
        {
            transform.Translate(1.2f * Time.deltaTime * -_dir);

            Collider[] players = Physics.OverlapBox(transform.position + new Vector3(0, 0.5369839f, 0), _size, Quaternion.identity, (1 << 3));

            foreach (Collider player in players)
            {
                Rigidbody rb = player.transform.parent.GetComponent<Rigidbody>();
                rb.MovePosition(player.transform.parent.position + 1.2f * Time.deltaTime * -_dir);
            }

            yield return new WaitForFixedUpdate();
        }

        transform.position = _originPosition;

        _script.MoveEnd += 1;  
    }

  public void StartMoveCoroutine()
  {
    StartCoroutine(StartMove());
  }

  public void ReturnMoveCoroutine()
  {
    StartCoroutine(ReturnMove());
  }
}
