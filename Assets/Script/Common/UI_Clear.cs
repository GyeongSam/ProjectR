

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using static WebManager;
using Unity.VisualScripting;

public class UI_Clear : MonoBehaviourPun
{
  public EggRandomGet erg;
  public GameObject ergPref;
  public Transform ergBox;
  public char gameType;
  private int N;
  private int checkN = 0;
  private int fullN = -1;
  private bool is_quit = false;

    private List<int> eggList;
  // Start is called before the first frame update
  public enum StageType
  {
    StageB,
    StageC,
    StageD,
    StageE
  }
  void Start()
  {
    N = PhotonNetwork.CurrentRoom.PlayerCount;

    for (int i = 1; i <= N; ++i)
    {
      for (int j = 0; j < 3; ++j)
      {
        transform.GetChild(5).transform.GetChild(0).transform.GetChild(i - 1).transform.GetChild(j).transform.GetChild(1).GetComponent<TMP_Text>().text = PhotonNetwork.CurrentRoom.Players[i].NickName;
      }
    }

    if (PhotonNetwork.IsMasterClient)
    {
      fullN = (1 << N) - 1;
    }
  }
  // ��� ������Ʈ ���� �Լ����� �� �������� �Ŵ������� ȣ���� ����
  //Ŭ���� ������ �޼����� �� ������ Ŭ���̾�Ʈ���� ȣ��(DB�� �� �ο� ���)
  // teamName = �� �̸�, teamId = team guid, memberIdList = ���� �ִ� �ο����� ���̵� ���
  

  // ��� ��� �Լ����� ������ ���� ���� �� Ŭ���� �ߴٸ� ȣ��
  //teamId = team�� guid, gameType = �Ϸ��� ������ ����, playTime = �ش� �������� �� �÷����� �ð�
  public void RecordUpdateForMastClient(string teamId, char gameType, int playTime) // �� ����� ������ Ŭ���̾�Ʈ�� ��û
  {
    WebManager.TeamRankingData teamRankingData = new WebManager.TeamRankingData();
    teamRankingData.teamId = teamId;
    teamRankingData.gameType = gameType;
    teamRankingData.playTime = playTime;

    StartCoroutine(WebManager.Instance().WebRequest("ranking/update-team-ranking", "POST", teamRankingData));
  }

  //teamId = team�� guid, gameType = �Ϸ��� ������ ����, playTime = �ش� �������� �� �÷����� �ð�, numberCoins = ���� ������ ��, getEggNum = Ŭ����� ȹ���� �� ĳ���� ��ȣ, memberId = "ȸ�� ���̵�(Player.Prefs���� ������ ��))
  public void RecordUpdate(string teamId, string memberId, int numberCoins, char gameType, int playTime) // ��� Ŭ���̾�Ʈ���� ȣ��
  {
    WebManager.PersonalRecordData personalRecordData = new WebManager.PersonalRecordData();
    personalRecordData.teamId = teamId;
    personalRecordData.memberId = memberId;
    personalRecordData.numberCoins = numberCoins;
    personalRecordData.gameType = gameType.ToString();
    personalRecordData.playTime = playTime;

    WebManager.PersonalRankingData personalRankingData = new WebManager.PersonalRankingData();
    personalRankingData.memberId = memberId;
    personalRankingData.gameType = gameType;
    personalRankingData.numberCoins = numberCoins;

    StartCoroutine(WebManager.Instance().WebRequest("member/update-personal-record", "POST", personalRecordData));
    StartCoroutine(WebManager.Instance().WebRequest("ranking/update-personal-ranking", "POST", personalRankingData));

    foreach (int num in eggList)
    {
        WebManager.MemberReqDto memberReqDto = new WebManager.MemberReqDto();
        memberReqDto.id = memberId;
        memberReqDto.nowEggNum = num;
        StartCoroutine(WebManager.Instance().WebRequest("member/get-egg", "POST", memberReqDto));
    }
  }

  [PunRPC]
  public void ReadyRPC(int playerN)
  {
    Transform temp;
    temp = transform.GetChild(5).transform.GetChild(0).transform.GetChild(playerN - 1).transform;
    temp.GetChild(0).gameObject.SetActive(false);
    temp.GetChild(1).gameObject.SetActive(true);

    if (PhotonNetwork.IsMasterClient)
    {
      checkN |= 1 << (playerN - 1);
      if (checkN == fullN)
      {
        photonView.RPC("RestartRPC", RpcTarget.AllViaServer);
      }
    }
  }

  [PunRPC]
  public void RestartRPC()
  {
    //�ӽ�
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
    //�ӽ�
  }

  [PunRPC]
  public void QuitRPC(int playerN)
  {
    Debug.Log(playerN);
    Transform temp;
    temp = transform.GetChild(5).transform.GetChild(0).transform.GetChild(playerN - 1).transform;
    temp.GetChild(1).gameObject.SetActive(false);
    temp.GetChild(2).gameObject.SetActive(true);
    if (PhotonNetwork.IsMasterClient)
    {
      checkN = -1;
      Debug.Log("���� ���� �Ұ���");
    }
  }

  public void getEgg(char gameType)
  {
     eggList = erg.eggRandomGet(gameType);
  }

  public void setUi_EggGet()
  {
    char[] temp = PlayerPrefs.GetString("MyEggList").ToCharArray();
    foreach (int item in eggList)
    {
      GameObject obj = Instantiate(ergPref);
      obj.transform.SetParent(ergBox, false);
      obj.GetComponent<GetEggIcon>().setEggInfo(item);
      WebManager.MemberReqDto memberReqDto = new WebManager.MemberReqDto();
      memberReqDto.id = PlayerPrefs.GetString("userName");
      memberReqDto.nowEggNum = item;
      StartCoroutine(WebManager.Instance().WebRequest("member/get-egg", "POST", memberReqDto));
      temp[item - 1] = '1';
    }
    string temp2 = new string(temp);
    PlayerPrefs.SetString("MyEggList", temp2);
  }


  public void stateRPC(string RPCname)
  {
    photonView.RPC(RPCname, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
    if (RPCname == "QuitRPC") PartyRPCHandler.Instance().JoinVillage();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Return))
    {
      stateRPC("ReadyRPC");
    }
    else if (Input.GetKeyDown(KeyCode.Escape))
    {
      stateRPC("QuitRPC");
      PartyRPCHandler.Instance().JoinVillage();
      // ���� ������
    }
  }

  IEnumerator UpdateTeamRanking(WebManager.TeamInfo teamInfo, WebManager.TeamRankingData teamRankingData)
  {
    yield return StartCoroutine(WebManager.Instance().WebRequest("team/add-team", "POST", teamInfo));

    StartCoroutine(WebManager.Instance().WebRequest("ranking/update-team-ranking", "POST", teamRankingData));
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
    while (!PhotonNetwork.InLobby) { yield return new WaitForSeconds(0.1f); }
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

}
