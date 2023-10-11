using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;

public class BombController : MonoBehaviourPun
{
  public GameObject bombObj;
  public GameObject explosionObj;
  public bool isGiantBomb;
  public StageDManager sm;
  //public StageDManager.MarkFloorWithGOCallback markCallback;

  //public Rigidbody bombRigidBody;
  // Start is called before the first frame update
  void Start()
  {
    //temp = bombObj.GetComponent<Renderer>().material.GetColor("_BaseColor");
  }

  private void OnEnable()
  {
    bombObj.SetActive(true);
    explosionObj.SetActive(false);
    bombObj.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1,1,1,1));
  }

  public void IgniteGranade(float timeToExplode)
  {
    StartCoroutine(RandomDelay(timeToExplode));
  }

  IEnumerator RandomDelay(float timeToExplode)
  {
    yield return new WaitForSeconds(timeToExplode);
    bombObj.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1, 0, 0, 1));
    StartCoroutine(Expolsion());
  }
  IEnumerator Expolsion()
  {
    yield return new WaitForSeconds(2f);
    bombObj.SetActive(false);
    explosionObj.SetActive(true);

    Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f);

    foreach (Collider collider in colliders)
    {
      if (collider.CompareTag("Player"))
      {
        collider.gameObject.GetComponent<PlayerController>().getHitByBomb(transform.position);
      }
      else if (isGiantBomb && PhotonNetwork.IsMasterClient && collider.CompareTag("Floor"))
      {
        collider.GetComponent<FloorScript>().dN(-1);
        //markCallback(collider.gameObject, StageDManager.MarkType.Destroy);
      }
    }
    sm.bombExp(isGiantBomb);
    yield return new WaitForSeconds(0.7f);
    gameObject.SetActive(false);

  }
}
