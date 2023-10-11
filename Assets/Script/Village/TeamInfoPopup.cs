using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;

public class TeamInfoPopup : MonoBehaviourPun
{
  public GameObject[] playerInTeamObjectList;
  public GameObject joinButtonPrefab;
  public GameObject leaveButtonPrefab;

  GameObject joinButton;
  GameObject leaveButton;

  public TMP_Text teamName;

  PartyManager.PartyActionFinishedCallback _joinPartyCallback;
  PartyManager.PartyActionFinishedCallback _leavePartyCallback;
  PartyRPCHandler partyRPCHandler;

  PartyManager.PartyEntity currentEntity;

  PartyManager partyManager;

  private void Start()
  {
    partyManager = transform.gameObject.AddComponent<PartyManager>();
    _joinPartyCallback = JoinPartyCallback;
    _leavePartyCallback = LeavePartyCallback;

    joinButton = Instantiate(joinButtonPrefab, transform);
    leaveButton = Instantiate(leaveButtonPrefab, transform);
    leaveButton.SetActive(false);
  }

  public void SetDetails(PartyManager.PartyEntity entity)
  {
    if (entity != null)
    {
      teamName.text = entity.title;
      foreach (var user in entity.users.Select((value, index) => new { value, index }))
      {
        GameObject go = playerInTeamObjectList[user.index];

        GameObject normalGameObject = go.transform.GetChild(0).gameObject;
        normalGameObject.SetActive(false);

        GameObject readyGameObject = go.transform.GetChild(1).gameObject;
        readyGameObject.SetActive(true);

        GameObject placeholder = readyGameObject.transform.GetChild(1).gameObject;
        TMP_Text text = placeholder.GetComponent<TMP_Text>();
        text.text = user.value;
      }
    }

    for (int i = entity == null ? 0 : entity.userCount; i < 6; i++)
    {
      GameObject go = playerInTeamObjectList[i];

      GameObject normalGameObject = go.transform.GetChild(0).gameObject;
      normalGameObject.SetActive(true);

      GameObject readyGameObject = go.transform.GetChild(1).gameObject;
      readyGameObject.SetActive(false);

      GameObject placeholder = readyGameObject.transform.GetChild(1).gameObject;
      TMP_Text text = placeholder.GetComponent<TMP_Text>();
      text.text = $"Player {i + 1}";
    }

    joinButton.SetActive(true);
    joinButton.GetComponent<Button>().onClick.RemoveAllListeners();
    joinButton.GetComponent<Button>().onClick.AddListener(() =>
    {
      string userName = PhotonNetwork.LocalPlayer.NickName;
      StartCoroutine(partyManager.JoinParty(entity.guid, userName, _joinPartyCallback));
    });
  }
  public void CreateRoom(string title, string userName, int temp)
  {
    StartCoroutine(partyManager.CreateParty(title,userName,temp,_joinPartyCallback));
  }

   void JoinPartyCallback(object entity)
  {
    SetDetails(entity as PartyManager.PartyEntity);
    currentEntity = entity as PartyManager.PartyEntity;
    PartyRPCHandler.Instance().SetCurrentEntity(entity as PartyManager.PartyEntity);

    joinButton.SetActive(false);

    leaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
    leaveButton.GetComponent<Button>().onClick.AddListener(() =>
    {
      string userName = PhotonNetwork.LocalPlayer.NickName;
      PartyManager.PartyEntity e = entity as PartyManager.PartyEntity;
      StartCoroutine(partyManager.LeaveParty(e.guid, userName, _leavePartyCallback));
    });
    leaveButton.SetActive(true);

    if ((entity as PartyManager.PartyEntity).userCount == 6)
    {
      PartyRPCHandler.Instance().JoinOrCreateRoom(entity as PartyManager.PartyEntity);
      StartCoroutine(partyManager.DeleteParty((entity as PartyManager.PartyEntity).guid));
    }

    return;
  }

  void LeavePartyCallback(object entity)
  {
    SetDetails(null);
    currentEntity = null;
    PartyRPCHandler.Instance().SetCurrentEntity(null);

    joinButton.SetActive(false);
    leaveButton.SetActive(false);

    joinButton.GetComponent<Button>().onClick.RemoveAllListeners();
    leaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
  }
}
