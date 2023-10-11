

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
  // 기록 업데이트 관련 함수들은 각 스테이지 매니저에서 호출할 예정
  //클리어 조건을 달성했을 때 마스터 클라이언트에서 호출(DB에 팀 인원 등록)
  // teamName = 팀 이름, teamId = team guid, memberIdList = 팀에 있는 인원들의 아이디 목록
  

  // 기록 등록 함수들은 게임이 끝이 났을 때 클리어 했다면 호출
  //teamId = team의 guid, gameType = 완료한 게임의 종류, playTime = 해당 스테이지 총 플레이한 시간
  public void RecordUpdateForMastClient(string teamId, char gameType, int playTime) // 팀 기록은 마스터 클라이언트만 요청
  {
    WebManager.TeamRankingData teamRankingData = new WebManager.TeamRankingData();
    teamRankingData.teamId = teamId;
    teamRankingData.gameType = gameType;
    teamRankingData.playTime = playTime;

    StartCoroutine(WebManager.Instance().WebRequest("ranking/update-team-ranking", "POST", teamRankingData));
  }

  //teamId = team의 guid, gameType = 완료한 게임의 종류, playTime = 해당 스테이지 총 플레이한 시간, numberCoins = 먹은 코인의 수, getEggNum = 클리어로 획득한 알 캐릭터 번호, memberId = "회원 아이디(Player.Prefs에서 가져올 것))
  public void RecordUpdate(string teamId, string memberId, int numberCoins, char gameType, int playTime) // 모든 클라이언트에서 호출
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
    Debug.Log(playerN);
    Transform temp;
    temp = transform.GetChild(5).transform.GetChild(0).transform.GetChild(playerN - 1).transform;
    temp.GetChild(1).gameObject.SetActive(false);
    temp.GetChild(2).gameObject.SetActive(true);
    if (PhotonNetwork.IsMasterClient)
    {
      checkN = -1;
      Debug.Log("게임 시작 불가능");
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
      // 게임 나가기
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
