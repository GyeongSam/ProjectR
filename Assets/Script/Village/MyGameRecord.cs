using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyGameRecord : MonoBehaviour
{
  public TMP_Text gameNameObj;
  public TMP_Text teamNameObj;
  public TMP_Text timeRecordObj;
  public TMP_Text coinRecordObj;
  public GameObject playerNameObj;
    // Start is called before the first frame update
  public void setInfo(string gameName, string teamName, string timeRecord, string coinRecord, string[] playerNames)
  {
    gameNameObj.text = gameName;
    teamNameObj.text = teamName;
    timeRecordObj.text = timeRecord;
    coinRecordObj.text = coinRecord;
    playerNameObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = teamName;
    playerNameObj.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = $"{playerNames[0]}\n{playerNames[1]}\n{playerNames[2]}";
    playerNameObj.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = $"{playerNames[3]}\n{playerNames[4]}\n{playerNames[5]}"; 
  }
  public void hoverOn()
  {
    playerNameObj.SetActive(true);
  }
  public void hoverOff()
  {
    playerNameObj.SetActive(false);
  }
}
