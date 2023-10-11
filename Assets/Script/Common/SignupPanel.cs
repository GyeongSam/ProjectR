using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SignupPanel : MonoBehaviour
{
    public GameObject signinPanel;

    TMP_InputField email;
    TMP_InputField id;
    TMP_InputField password;

    private int next_focus = 0;
    // Start is called before the first frame update
    private void OnEnable()
    {
        transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(false);

        email = transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_InputField>();
        id = transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TMP_InputField>();
        password = transform.GetChild(1).GetChild(2).GetChild(2).GetComponent<TMP_InputField>();

        nextFocus();
    }

    public void API_Signup()
    {
        // Signup api ø‰√ª
        StartCoroutine(SignUp(id.text, password.text, email.text));
    }
    public void nextFocus()
    {
        transform.GetChild(1).transform.GetChild(2).transform.GetChild(next_focus).GetComponent<Selectable>().Select();
    }
    public void editNextIdx(int next_idx)
    {
        next_focus = next_idx;
    }

    public void back()
    {
        signinPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            API_Signup();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            back();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            nextFocus();
        }
    }

    IEnumerator SignUp(string id, string password, string email)
    {
        WebManager.MemberReqDto memberReqDto = new WebManager.MemberReqDto();
        memberReqDto.id = id;
        memberReqDto.password = password;
        memberReqDto.email = email;

        yield return StartCoroutine(WebManager.Instance().WebRequest("member/join", "POST", memberReqDto));

        bool.TryParse(WebManager.Instance().WebRequestResult, out bool result);

        if (result)
        {
            back();
        }

        else
        {
            transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(true);
        }
    }
}
