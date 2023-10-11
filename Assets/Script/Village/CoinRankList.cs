using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CoinRankList : MonoBehaviour
{
  public TMP_Text userName;
  public TMP_Text coinRecord;
  public Transform medals;
  public TMP_Text medal_num;

  public void setInfo(string user, string coin, int n)
  {
    userName.text = user;
    coinRecord.text = coin;
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
