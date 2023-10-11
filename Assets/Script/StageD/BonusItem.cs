using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BonusItem : MonoBehaviourPun, IInstanceManager
{
  public StageDManager sm;
  private bool onDelay = true;
  // Start is called before the first frame update
  void Start()
  {
    sm = GameObject.Find("StageManager").GetComponent<StageDManager>();
  }
  void OnEnable()
  {
    onDelay = true;
    StartCoroutine(Delay());
  }
  [PunRPC]
  void disable()
  {
    gameObject.SetActive(false);
  }
  IEnumerator Delay()
  {
    yield return new WaitForSeconds(0.3f);
    onDelay = false;
    yield return new WaitForSeconds(5f);
    disable();

  }
  // Update is called once per frame
  void Update()
  {
    transform.Rotate(new Vector3(0, 1, 0) * 180 * Time.deltaTime);
  }

 

  private void OnTriggerEnter(Collider other)
  {
    if (onDelay) { return; }
    if (other.CompareTag("Player")&& other.gameObject.GetComponent<PlayerController>().photonView.IsMine)
    {
      sm.scoreUp();
      photonView.RPC("disable", RpcTarget.AllViaServer);
    }
  }

}
