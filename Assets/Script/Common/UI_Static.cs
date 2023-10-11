using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Static : MonoBehaviour
{
  TMP_Text coins;
  TMP_Text time;
  Sprite MyEggImg;
  public TMP_Text userNameBox;
  public TMP_Text userLevelBox;
  public Transform eggProfileBox;
  public int s=0;
  public int m=0;
  string s_="00";
  string m_="00";
  
    // Start is called before the first frame update
    void Start()
    {
    //로그인 API 달기 전임시용
    
    userNameBox.text = PlayerPrefs.GetString("userName");
    userLevelBox.text = PlayerPrefs.GetInt("level").ToString(); 

    setMyEgg();
      time = transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
      coins = transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<TMP_Text>();
    coins.text = 0.ToString();
      //transform.GetChild(1).transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).GetComponent<TMP_Text>().text= 11.ToString();
      StartCoroutine(TimeCRT());
      

    }


  public void setMyEgg(){
    if (eggProfileBox.childCount>0) Destroy(eggProfileBox.GetChild(0).gameObject);
      GameObject obj = Instantiate(Resources.Load($"Village/Egg{PlayerPrefs.GetInt("nowEggNum")}") as GameObject);
      obj.transform.SetParent(eggProfileBox, false);
      obj.transform.localScale = new Vector3(obj.transform.localScale.x * 1.3f, obj.transform.localScale.y * 1.3f, obj.transform.localScale.z * 0.3f);
   }

  public void setCoin(int coin)
  {
    coins.text = coin.ToString();
  }


    IEnumerator TimeCRT()
  {
    yield return new WaitForSeconds(1f);
    StartCoroutine(TimeCRT());
    if (++s == 60) { m++; s = 0; m_ = m < 10 ? "0" + m.ToString() : m.ToString(); }
    s_ = s < 10 ? "0" + s.ToString() : s.ToString();
    time.text = m_ + ":" + s_;
  }
}
