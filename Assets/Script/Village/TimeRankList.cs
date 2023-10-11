using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeRankList : MonoBehaviour
{
  public TMP_Text teamNameBox;
  public TMP_Text playerList1;
  public TMP_Text playerList2;
  public TMP_Text timeRecordBox;
  public Transform medals;
  public TMP_Text medal_num;
  
  public void setInfo(string teamName, string timeRecord, string[] playerNames, int n)
  {
    teamNameBox.text = teamName;
    timeRecordBox.text = timeRecord;
    playerList1.text = $"{playerNames[0]}\n{playerNames[1]}\n{playerNames[2]}";
    playerList2.text = $"{playerNames[3]}\n{playerNames[4]}\n{playerNames[5]}";
    if (n < 4)
    {
      medals.GetChild(n).gameObject.SetActive(true);
    }
    else
    {
      medals.GetChild(4).gameObject.SetActive(true);
      medal_num.text = n.ToString();
    }

  }
}
