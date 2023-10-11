
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using static MyNetworkManager;

public class UI_Fail : MonoBehaviourPun
{
  public enum StageType
  {
    StageB,
    StageC,
    StageD,
    StageE
  }
  private int N;
  private int checkN = 0;
  private int fullN = -1;
  private bool is_quit = false;

  public GameObject readyGroup;
  // Start is called before the first frame update
  void Start()
  {
    N = PhotonNetwork.CurrentRoom.PlayerCount;

    for (int i = 1; i<=N; ++i)
    {
      for (int j = 0; j<3; ++j)
      {
        readyGroup.transform.GetChild(i - 1).transform.GetChild(j).transform.GetChild(1).GetComponent<TMP_Text>().text = PhotonNetwork.CurrentRoom.Players[i].NickName;
      }
    }

    if (PhotonNetwork.IsMasterClient)
    {
      fullN = (1 << N) - 1;
    }
  }

  [PunRPC]
  public void ReadyRPC(int playerN)
  {
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(0).gameObject.SetActive(false);
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(1).gameObject.SetActive(true);
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(2).gameObject.SetActive(false);

    if (PhotonNetwork.IsMasterClient)
    {
      checkN |= 1 << (playerN - 1);
      if (checkN == fullN)
      {
        Debug.Log("리트 시작 하겠습니다!!!!");
        photonView.RPC("RestartRPC", RpcTarget.AllViaServer);
      }
    }
  }
  [PunRPC]
  public void RestartRPC()
  {
    //임시
    switch ((int)PhotonNetwork.CurrentRoom.CustomProperties["StageType"])
    {
      case (int)StageType.StageB:
        PhotonNetwork.LoadLevel(1);
        break;
      case (int)StageType.StageC:
        PhotonNetwork.LoadLevel(4);
        break;
      case (int)StageType.StageD:
        PhotonNetwork.LoadLevel(2);
        break;
      case (int)StageType.StageE:
        PhotonNetwork.LoadLevel(3);
        break;
    }
    //임시
  }

  [PunRPC]
  public void QuitRPC(int playerN)
  {
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(0).gameObject.SetActive(false);
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(1).gameObject.SetActive(false);
    readyGroup.transform.GetChild(playerN - 1).transform.GetChild(2).gameObject.SetActive(true);
    if (PhotonNetwork.IsMasterClient)
    {
      checkN = -1;
      Debug.Log("게임 시작 불가능");
    }
  }

  public void stateRPC(string RPCname)
  {

    Debug.Log(RPCname);
    Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
    Debug.Log(RpcTarget.AllViaServer);
    photonView.RPC(RPCname, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
    if (RPCname == "QuitRPC")PartyRPCHandler.Instance().JoinVillage();
  }

  public void villageWait()
  {
    StartCoroutine(VillageWaiter());
  }
  public void lobbyWait()
  {
    StartCoroutine(LobbyWaiter());
  }
  IEnumerator VillageWaiter()
  {
    Debug.Log("4");

    while (!PhotonNetwork.InLobby) { yield return new WaitForSeconds(0.1f); }
    Debug.Log("42");
    GameObject.Find("Main Camera").AddComponent<MyNetworkManager>();
    MyNetworkManager mnm = GameObject.Find("Main Camera").GetComponent<MyNetworkManager>();
    mnm.JoinRoom("Village");
  }
  IEnumerator LobbyWaiter()
  {
    while (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
    {
      if (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
        MyNetworkManager.Instance().JoinLobby();

      yield return new WaitForSeconds(3f);
    }
  }
    public void QuitPopUp(bool open)
  {
    is_quit = open;
    transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(open);
  }

  // Update is called once per frame
  void Update()
  {
    if (!is_quit)
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        stateRPC("ReadyRPC");
      }
      else if (Input.GetKeyDown(KeyCode.Escape))
      {
        QuitPopUp(true);
      }
    }
    else
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        stateRPC("QuitRPC"); 
      }
      else if (Input.GetKeyDown(KeyCode.Escape))
      {
        QuitPopUp(false);
      }
    }
    
  }
}
