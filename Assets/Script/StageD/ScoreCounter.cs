using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public StageDManager sm;
    private TMP_Text score_text;
    // Update is called once per frame
    void Start()
    {
        score_text = GetComponent<TMP_Text>();
    }
    void Update()
    {
        score_text.text = sm.gameScore.ToString() + "coins";
    }
}
