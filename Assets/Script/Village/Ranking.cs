using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

public class Ranking : MonoBehaviour
{
    public TMP_Text gameNameBox;
    public string gameName;
    public GameObject timeRanking;
    public GameObject coinRanking;
    public GameObject timeRankListPrefab;
    public GameObject coinRankingPrefab;

    private void OnEnable()
    {
        //API ∑©≈∑ ø‰√ª
        gameNameBox.text= gameName;

        StartCoroutine(GetRankingInfo());     
    }

    private void OnDisable()
    {
        for(int i = 0; i < timeRanking.transform.childCount; i++)
        {
            Destroy(timeRanking.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < coinRanking.transform.childCount; i++)
        {
            Destroy(coinRanking.transform.GetChild(i).gameObject);
        }
    }

    public void close()
    {
        gameObject.SetActive(false);
    GameObject.Find("UI").transform.GetChild(0).gameObject.SetActive(true);
  }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            close();
        }
    }

    IEnumerator GetRankingInfo()
    {
        yield return StartCoroutine(WebManager.Instance().WebRequest($"ranking/get-team-ranking/{gameName}", "GET", null));

        string jsonArrayStr = "{\"rankingList\" :" + WebManager.Instance().WebRequestResult + "}";
        WebManager.TeamRanking teamRankingRespDto = JsonConvert.DeserializeObject<WebManager.TeamRanking>(jsonArrayStr);

    int n = 1;
        foreach (WebManager.TeamRankingData data in teamRankingRespDto.rankingList)
        {
            GameObject objTime = Instantiate(timeRankListPrefab);
            objTime.transform.SetParent(timeRanking.transform, false);

            yield return StartCoroutine(WebManager.Instance().WebRequest($"team/get-team-info?teamId={data.teamId}", "GET", null));
            WebManager.TeamInfo teamInfo = JsonConvert.DeserializeObject<WebManager.TeamInfo>(WebManager.Instance().WebRequestResult);

            string[] temp = { teamInfo.memberIdList[0], teamInfo.memberIdList[1], teamInfo.memberIdList[2], teamInfo.memberIdList[3], teamInfo.memberIdList[4], teamInfo.memberIdList[5] };
            objTime.GetComponent<TimeRankList>().setInfo(data.teamName, $"{data.playTime / 60}:{data.playTime % 60}", temp, n++);
        }

        yield return StartCoroutine(WebManager.Instance().WebRequest($"ranking/get-personal-ranking/{gameName}", "GET", null));

        jsonArrayStr = "{\"rankingList\" :" + WebManager.Instance().WebRequestResult + "}";
        WebManager.PersonalRanking personalRankingRespDto = JsonConvert.DeserializeObject<WebManager.PersonalRanking>(jsonArrayStr);

    n = 1;
        foreach (WebManager.PersonalRankingData data in personalRankingRespDto.rankingList)
        {
            GameObject objCoin = Instantiate(coinRankingPrefab);
            objCoin.transform.SetParent(coinRanking.transform, false);

            objCoin.GetComponent<CoinRankList>().setInfo(data.memberId, data.numberCoins.ToString(), n++);
        }
    }
}
