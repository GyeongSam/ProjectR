using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Windows;

public class PartyManager : MonoBehaviour
{
  public GameObject createButton;
  public GameObject createPopUp;
  public class PartyEntity
  {
    public string guid;
    public string title;
    public int type;
    public List<string> users;
    public string leaderId;
    public int userCount;

  }
  private void Start()
  {
    createButton = GameObject.Find("UI").transform.GetChild(3).transform.GetChild(1).transform.GetChild(5).gameObject;
    createPopUp = GameObject.Find("UI").transform.GetChild(4).gameObject;
  }


  private List<PartyEntity> partyList;
  private PartyEntity currentParty;

  public delegate void PartyActionFinishedCallback(object param);

  public IEnumerator GetPartyList(PartyActionFinishedCallback _cb = null)
  {
    yield return StartCoroutine(WebManager.Instance().WebRequest("party/list", "GET", null));
    try
    {
      partyList = JsonConvert.DeserializeObject<List<PartyEntity>>(WebManager.Instance().WebRequestResult);
    }
    catch { }

    if(_cb != null)
      _cb(partyList);
  }

  public void cp(string title, string userName, int temp)
  {
    StartCoroutine(CreateParty(title, userName, temp));
  }
  public IEnumerator CreateParty(string title, string userId, int type, PartyActionFinishedCallback _cb = null)
  {

    WebManager.CreatePartyReq createPartyReq = new WebManager.CreatePartyReq();
    createPartyReq.title = title;
    createPartyReq.leaderId = userId;
    createPartyReq.type = type;

    yield return StartCoroutine(WebManager.Instance().WebRequest("party/create", "POST", createPartyReq));
    Debug.Log(WebManager.Instance().WebRequestResult);
    try
    {
      currentParty = JsonConvert.DeserializeObject<PartyEntity>(WebManager.Instance().WebRequestResult);
      Debug.Log("1");
      createButton.SetActive(false);
      Debug.Log("2");
      createPopUp.SetActive(false);
      Debug.Log("3");
      createPopUp.transform.GetChild(1).transform.GetChild(3).gameObject.SetActive(false);
      Debug.Log("4");

    }
    catch {

      Debug.Log("Fail");
      createPopUp.transform.GetChild(1).transform.GetChild(3).gameObject.SetActive(true);
    }

    

    if (_cb != null)
    {
      _cb(currentParty);
    }
  }

  public IEnumerator JoinParty(string guid, string userId, PartyActionFinishedCallback _cb = null)
  {
    WebManager.JoinPartyReq joinPartyReq = new WebManager.JoinPartyReq();
    joinPartyReq.guid = guid;
    joinPartyReq.userId = userId;

    yield return StartCoroutine(WebManager.Instance().WebRequest("party/join", "POST", joinPartyReq));
    try
    {
      currentParty = JsonConvert.DeserializeObject<PartyEntity>(WebManager.Instance().WebRequestResult);
      createButton.SetActive(false);
    }
    catch { }


    if (_cb != null)
    {
      _cb(currentParty);
    }
      
  }

  public IEnumerator LeaveParty(string guid, string userId, PartyActionFinishedCallback _cb = null)
  {
    WebManager.LeavePartyReq leavePartyReq = new WebManager.LeavePartyReq();
    leavePartyReq.guid = guid;
    leavePartyReq.userId = userId;
    yield return StartCoroutine(WebManager.Instance().WebRequest("party/leave", "POST", leavePartyReq));
    createButton.SetActive(true);
    currentParty = null;

    if (_cb != null)
    {
      _cb(currentParty);
    }
      
  }

  public IEnumerator DeleteParty(string guid, PartyActionFinishedCallback _cb = null)
  {
    yield return StartCoroutine(WebManager.Instance().WebRequest("party/delete", "DELETE", guid));
    createButton.SetActive(true);
    currentParty = null;

    if (_cb != null)
    _cb(currentParty);
  }

  /*public IEnumerator ChangeAttribute(string guid, string title = null, string leaderId = null, int type = -1)
  {
    WebManager.ChangePartyAttributeReq changePartyAttributeReq = new WebManager.ChangePartyAttributeReq();
    changePartyAttributeReq.guid = guid;
    changePartyAttributeReq.title = title == null ? currentParty.title : title;
    changePartyAttributeReq.leaderId = leaderId == null ? currentParty.leaderId : leaderId;
    changePartyAttributeReq.type = type == -1 ? currentParty.type : type;

    // change controller needed
    //yield return StartCoroutine(WebManager.Instance().WebRequest("party/join", "POST", changePartyAttributeReq));
  }*/
}
