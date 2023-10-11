using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RoomList : MonoBehaviour
{
  public TMP_Text roomName;
  public TMP_Text roomNum;
  public GameObject buttonWrapper;

  public GameObject _partyInfo;

  public void SetInfo(PartyManager.PartyEntity entity, GameObject partyInfo, GameObject instanceEntity)
  {
    roomName.text = entity.title;
    roomNum.text = entity.userCount.ToString();

    _partyInfo = partyInfo;

    Transform entityTransform = instanceEntity.transform;
    entityTransform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { SetPopupDetails(entity); });
  }

  //void SetPopupDetails(PartyManager.PartyEntity entity)
  public void SetPopupDetails(PartyManager.PartyEntity entity)
  {
    TeamInfoPopup teamInfoPopup = _partyInfo.GetComponent<TeamInfoPopup>();
    teamInfoPopup.SetDetails(entity);
  }
}
