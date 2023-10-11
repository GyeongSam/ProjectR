using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Text;
using Newtonsoft.Json;

public class MyInfo : MonoBehaviour
{
  public EggInfo egginfo;
  public TMP_Text username;
  public TMP_Text level;
  public TMP_Text eggCountText;
  private int eggCount = 0;
  public int nowEgg;
  private int nowPage = 0;
  private int changeEggNum = 0;
  private bool isPopUpOn = false;
  public GameObject eggChangePopUp;
  public GameObject eggPages;
  public GameObject eggPagesNavi;
  public GameObject gameRecordBox;
  bool[] eggList = {false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false ,
                        false,false, false, false, false, false, false, false, false, false, false,false, false, false, false, false, false, false, false, false };
  int selectedEggNum = 0;
  GameObject nowEggObject;
  public void closeMyInfo()
  {
    gameObject.SetActive(false);
    GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
  }

  public void chagnePage(int n)
  {
    for (int i = 0; i < 4; ++i)
    {
      if (i == n) continue;
      eggPages.transform.GetChild(i).gameObject.SetActive(false);
      eggPagesNavi.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
    }
    eggPages.transform.GetChild(n).gameObject.SetActive(true);
    eggPagesNavi.transform.GetChild(n).transform.GetChild(0).gameObject.SetActive(true);
    nowPage = n;
    isPopUpOn = false;
    closeEggChangePopUp();
  }
  public void chagnePageLeft()
  {
    if (nowPage == 0) return;
    chagnePage(nowPage - 1);
  }
  public void chagnePageRight()
  {
    if (nowPage == 3) return;
    chagnePage(nowPage + 1);
  }

  public void setEggChangePopUp()
  {
    GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
    if (clickedObj != null && clickedObj.name[0] == 'E' && clickedObj.name[1] == 'g' && clickedObj.name[2] == 'g')
    {
      closeEggChangePopUp();
      selectedEggNum = int.Parse(clickedObj.name.Substring(3));
      Debug.Log(clickedObj.name);
      eggChangePopUp.transform.GetChild(2).GetComponent<TMP_Text>().text = clickedObj.name;
      eggChangePopUp.transform.GetChild(4).GetComponent<TMP_Text>().text = "Grade : " + egginfo.EggGradeList[selectedEggNum] + "\n\nDescription :\n" + egginfo.EggDescription[selectedEggNum];

      GameObject obj = Instantiate(Resources.Load($"Village/{clickedObj.name}") as GameObject, eggChangePopUp.transform);
      obj.transform.localScale *= 1.5f;
      obj.transform.localPosition = new Vector3(obj.transform.localPosition.x - 100f, obj.transform.localPosition.y, obj.transform.localPosition.z);

      eggChangePopUp.SetActive(true);
      isPopUpOn = true;
      if (!eggList[Convert.ToInt32(clickedObj.name.Substring(3)) - 1])
      {
        changeEggNum = 0;
        obj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.black;
        eggChangePopUp.transform.GetChild(3).gameObject.SetActive(false);
      }
      else
      {
        changeEggNum = Convert.ToInt32(clickedObj.name.Substring(3));
        eggChangePopUp.transform.GetChild(3).gameObject.SetActive(true);
      }
    }
  }
  public void closeEggChangePopUp()
  {
    selectedEggNum = 0;

    if (eggChangePopUp.transform.childCount > 5)
    {
      Destroy(eggChangePopUp.transform.GetChild(5).gameObject);
    }
    eggChangePopUp.SetActive(false);
  }
  public void eggChange()
  {
    StartCoroutine(ChangeEgg(selectedEggNum));
  }
  public void setEggProfile()
  {
    if (nowEggObject != null) Destroy(nowEggObject);
    nowEggObject = Instantiate(Resources.Load($"Village/Egg{nowEgg}") as GameObject, transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).transform);
    nowEggObject.transform.localScale *= 2;
  }

  private void OnEnable()
  {
    nowEgg = PlayerPrefs.GetInt("nowEggNum");
    eggCount = 0;
    setEggProfile();
    username.text = PlayerPrefs.GetString("userName");

    StartCoroutine(GetMyPage());
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (isPopUpOn)
      {
        isPopUpOn = false;
        closeEggChangePopUp();
      }
      else closeMyInfo();
    }
    if (Input.GetMouseButtonDown(0))
    {
      setEggChangePopUp();
    }
  }

    private void OnDisable()
    {
        for (int i = 0; i < gameRecordBox.transform.childCount; i++)
        {
            Destroy(gameRecordBox.transform.GetChild(i).gameObject);
        }
        if (nowEggObject != null) Destroy(nowEggObject);

        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 28; ++j)
            {
                if (i == 3 && j > 15) break;
                Destroy(eggPages.transform.GetChild(i).transform.GetChild(j).GetChild(1).gameObject);
            }
        }

        if (isPopUpOn)
        {
            isPopUpOn = false;
            closeEggChangePopUp();
        }
    }

    IEnumerator GetMyPage()
  {
    String eggPossession = PlayerPrefs.GetString("MyEggList");
    
    for(int i = 0; i < eggPossession.Length; i++)
    {
        if (eggPossession[i] == '1')
            eggList[i] = true;
    }

    for (int i = 0; i < 4; ++i)
    {
      for (int j = 0; j < 28; ++j)
      {
        if (i == 3 && j > 15) break;
        GameObject prefab = Resources.Load($"Village/Egg{i * 28 + j + 1}") as GameObject;
        GameObject obj = Instantiate(prefab, eggPages.transform.GetChild(i).transform.GetChild(j).transform);
        obj.transform.AddComponent<Selectable>();
        if (eggList[i * 28 + j])
        {
          eggCount++;
        }
        else
        {
          obj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.black;
        }
      }
    }


    level.text = "Lv." + eggCount.ToString();
    eggCountText.text = eggCount.ToString() + "/100";

    yield return StartCoroutine(WebManager.Instance().WebRequest($"member/get-personal-record/{username.text}", "GET", null));

    string jsonArrayStr = "{\"recordList\" :" + WebManager.Instance().WebRequestResult + "}";
    WebManager.PersonalRecord personalRecord = JsonConvert.DeserializeObject<WebManager.PersonalRecord>(jsonArrayStr);

    foreach (WebManager.PersonalRecordData personalRecordData in personalRecord.recordList)
    {
      GameObject tempObj = Instantiate(Resources.Load("Village/List") as GameObject);
      tempObj.transform.SetParent(gameRecordBox.transform, false);

      yield return StartCoroutine(WebManager.Instance().WebRequest($"team/get-team-info?teamId={personalRecordData.teamId}", "GET", null));
      WebManager.TeamInfo teamInfo = JsonConvert.DeserializeObject<WebManager.TeamInfo>(WebManager.Instance().WebRequestResult);

      string[] temp = { teamInfo.memberIdList[0], teamInfo.memberIdList[1], teamInfo.memberIdList[2], teamInfo.memberIdList[3], teamInfo.memberIdList[4], teamInfo.memberIdList[5] };
      tempObj.GetComponent<MyGameRecord>().setInfo("Stage " + personalRecordData.gameType, personalRecordData.teamName, $"{personalRecordData.playTime / 60}:{personalRecordData.playTime % 60}", personalRecordData.numberCoins.ToString(), temp);
    }
  }
  public void changeEgg()
  {
    if (changeEggNum != 0) StartCoroutine(ChangeEgg(changeEggNum));
  }
  IEnumerator ChangeEgg(int selectedEggNum)
  {
    WebManager.MemberReqDto memberReqDto = new WebManager.MemberReqDto();
    memberReqDto.id = PlayerPrefs.GetString("userName");
    memberReqDto.nowEggNum = selectedEggNum;

    yield return StartCoroutine(WebManager.Instance().WebRequest("member/change-egg", "POST", memberReqDto));

    if (bool.Parse(WebManager.Instance().WebRequestResult))
    {
      nowEgg = selectedEggNum;
      PlayerPrefs.SetInt("nowEggNum", nowEgg);
      setEggProfile();

      GameObject.Find("UI").transform.GetChild(0).GetComponent<UI_Static>().setMyEgg();

      GameObject[] P = GameObject.FindGameObjectsWithTag("Player");
      foreach (GameObject p in P)
      {
        if (p.GetComponent<PlayerControllerVillage>().photonView.IsMine)
        {
          p.GetComponent<PlayerControllerVillage>().ChangeEgg(selectedEggNum);
          break;
        }
      }

    }
  }
}
