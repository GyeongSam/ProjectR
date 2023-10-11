using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

public class WebManager
{
  private static WebManager instance;

  public static WebManager Instance()
  {
      if(instance == null)
          instance = new WebManager();
      return instance;
  }

  readonly string baseUrl = "http://j8a806.p.ssafy.io:8080/";

  [Serializable]
  public class MemberReqDto
  {
    public string id;
    public string password;
    public string email;
    public int nowEggNum;
  }

  [Serializable]
  public class LoginRespDto
  {
    public string id;
    public bool successFailure;
    public int nowEgg;
  }

  [Serializable]
  public class EggPossessionRespDto
  {
    public int[] eggPossessionInfo;
  }

  [Serializable]
  public class PersonalRecord
  {
    public PersonalRecordData[] recordList;
  }

  [Serializable]
  public class PersonalRecordData
  {
    public string memberId;
    public string gameType;
    public int playTime;
    public string recordTime;
    public int numberCoins;
    public string teamId;
    public string teamName;
  }

  [Serializable]
  public class TeamRanking
  {
    public TeamRankingData[] rankingList;
  }

  [Serializable]
  public class TeamRankingData
  {
    public int playTime;
    public string teamName;
    public string teamId;
    public char gameType;
  }

  [Serializable]
  public class PersonalRanking
  {
    public PersonalRankingData[] rankingList;
  }

  [Serializable]
  public class PersonalRankingData
  {
    public int numberCoins;
    public string memberId;
    public char gameType;
  }

  [Serializable]
  public class TeamInfo
  {
    public string teamId;
    public string teamName;
    public List<string> memberIdList;
  }

  [Serializable]
  public class JoinPartyReq
  {
    public string guid;
    public string userId;
  }

  [Serializable]
  public class CreatePartyReq
  {
    public string title;
    public string leaderId;
    public int type;
  }

  [Serializable]
  public class LeavePartyReq
  {
    public string guid;
    public string userId;
  }

  [Serializable]
  public class DeletePartyReq
  {
    public string guid;
  }

  string _webRequestResult;

  public string WebRequestResult {
    get { return _webRequestResult; }
    set { _webRequestResult = value; }
  }

  public IEnumerator WebRequest(string url, string method, object o)
  {
    UnityWebRequest uwr = new UnityWebRequest(baseUrl + url, method);

    if (o != null)
    {
      byte[] jsonBytes = null;
      string jsonStr = JsonUtility.ToJson(o);
      jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
      uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
      uwr.SetRequestHeader("Content-Type", "application/json");
    }
    uwr.downloadHandler = new DownloadHandlerBuffer();

    yield return uwr.SendWebRequest();

    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
      _webRequestResult = "network Error";

    else
      _webRequestResult = uwr.downloadHandler.text;

    uwr.Dispose();
  }
}
