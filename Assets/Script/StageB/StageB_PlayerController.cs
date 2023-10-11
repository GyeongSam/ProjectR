using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class StageB_PlayerController : MonoBehaviourPun
{
  public static GameObject LocalPlayerInstance;
  public float moveForce = 5.0f;
  private Rigidbody rigidBody;
  public bool stuned = false;
  private float stunedTime = 3f;
  private float biggerTime = 10f;
  // Start is called before the first frame update
  void Start()
  {
    rigidBody = GetComponent<Rigidbody>();
    if (!photonView.IsMine) return;
    GameObject.Find("me").GetComponent<MyChar>().obj = transform;
  }
  // Update is called once per frame
  public void SpeedChange(float c)
  {
    if (photonView.IsMine)
    {
      moveForce += c;
    }
  }
  public void GetBigger(int c)
  {
    if (photonView.IsMine)
    {
      transform.localScale *= c;
      rigidBody.mass *= c * c;
      moveForce *= c * c;
      StartCoroutine(GetSmaller(c));
    }
  }
  public void GetStuned()
  {
    if (photonView.IsMine)
    {
      stuned = true;
      StartCoroutine(restoration());
    }
  }
  IEnumerator restoration()
  {
    if (photonView.IsMine)
    {
      yield return new WaitForSeconds(stunedTime);
      stuned = false;
    }
  }
  IEnumerator GetSmaller(int c)
  {
    if (photonView.IsMine)
    {
      yield return new WaitForSeconds(biggerTime);
      float temp = (float)1 / c;
      Debug.Log(temp);
      transform.localScale *= temp;
      rigidBody.mass *= temp * temp;
      moveForce *= temp * temp;
    }
  }




  public void Update()
  {
    if (!photonView.IsMine || stuned)
      return;

    if (Input.GetKey(KeyCode.W))
    {
      rigidBody.AddForce(Vector3.forward * moveForce);
    }
    if (Input.GetKey(KeyCode.D))
    {
      rigidBody.AddForce(Vector3.right * moveForce);
    }
    if (Input.GetKey(KeyCode.S))
    {
      rigidBody.AddForce(Vector3.back * moveForce);
    }
    if (Input.GetKey(KeyCode.A))
    {
      rigidBody.AddForce(Vector3.left * moveForce);
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!photonView.IsMine) return;
    if (collision.collider.CompareTag("Fail"))
    {
      Debug.Log("실패 인식함");
      GameObject.Find("StageManager").GetComponent<StageBManager>().UiControll();
    }
    if (collision.collider.CompareTag("Floor"))
    {
      stuned = true;
    }
  }
}
