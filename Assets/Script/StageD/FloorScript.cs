using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Photon.Pun;

public class FloorScript : MonoBehaviourPun
{
    public int N;
    public bool isOn = false;
    public int num;
    TMP_Text numText;

    void Start()
    {
        numText = gameObject.GetComponentInChildren<TMP_Text>();
        numText.text = N.ToString();
    }
    void Update()
    {
        if (isOn)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size + new Vector3(0, 40, 0), Quaternion.identity);
            num = 0;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Player") && collider.gameObject.GetComponent<PlayerController>().is_floor)
                {
                    num++;
                }
            }
            if (num > N)
            {
                gameObject.SetActive(false);
            }
            else numText.text = (N - num).ToString();
        }
        else
        {
            num = 0;
            numText.text = N.ToString();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isOn = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isOn = false;
        }
    }

  //public void UpgradeFloor()
  //{
  //  int viewID = this.photonView.ViewID;
  //  photonView.RPC("UpgradeFloorRPC", RpcTarget.AllViaServer, viewID);
  //}

  //[PunRPC]
  //public void UpgradeFloorRPC(int viewID)
  //{
  //  if(this.photonView.ViewID == viewID)
  //  {
  //    this.N++;
  //  }
  //}

  public void dN(int c)
  {
    photonView.RPC("dNRPC", RpcTarget.AllViaServer, c);
  }
  [PunRPC]
  public void dNRPC(int c)
  {
    N += c;
    if (N <= 0)
    {
      gameObject.SetActive(false);
      Collider[] colliders = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size + new Vector3(0, 40, 0), Quaternion.identity);
      num = 0;
      foreach (Collider collider in colliders)
      {
        if (collider.gameObject.CompareTag("Player"))
        {
          collider.gameObject.GetComponent<PlayerController>().is_jump = true;
        }
      }
    }
  }
}
