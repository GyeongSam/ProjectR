using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UpgradeItem : MonoBehaviourPun, IInstanceManager
{
  private GameObject itemButton;
  private bool onDelay = true;
  public Transform itembutton;

  // Start is called before the first frame update
  private void OnEnable()
  {
    onDelay = true;
    StartCoroutine(Delay());
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
    transform.Rotate(new Vector3(0, 1, 0) * 90 * Time.deltaTime, Space.World);
  }

  [PunRPC]
  void disable()
  {
    gameObject.SetActive(false);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (onDelay) return;

    Debug.Log("충돌");
    if (other.CompareTag("Player")&&other.gameObject.GetComponent<PlayerController>().photonView.IsMine)
    {

      Debug.Log("플레이어다");
      other.gameObject.GetComponent<PlayerController>().canUseUpgradeItem = true;
      itembutton.GetChild(0).gameObject.SetActive(true);
      itembutton.GetChild(1).gameObject.SetActive(false);
      disable();
      photonView.RPC("disable", RpcTarget.AllViaServer);
    }

  }


}
