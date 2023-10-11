using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class StageB_UI_Fail : MonoBehaviourPun
{
  private int N;
  private int checkN=0;
  private int fullN=-1;

    // Start is called before the first frame update
    void Start()
    {
      N = PhotonNetwork.CurrentRoom.PlayerCount;
      if (PhotonNetwork.IsMasterClient)
      {
        fullN = (1 << N) - 1;
      }
    }
  [PunRPC]
  public void ReadyRPC(int playerN)
  {
    transform.GetChild(playerN - 1).GetComponent<Image>().color = Color.green;
    if (PhotonNetwork.IsMasterClient)
    {
      checkN |= 1 << (playerN - 1);
      if (checkN == fullN) Debug.Log("리트 시작 하겠습니다!!!!");
    }
  }
    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.Return)) 
      {
        photonView.RPC("ReadyRPC", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
    }
      else if (Input.GetKeyDown(KeyCode.Escape)) 
      {
        
      }
    }
}
