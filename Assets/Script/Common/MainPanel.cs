using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    private TMP_Text c;
    public GameObject Popup_Login;
    // Start is called before the first frame update
    void Start()
    {
        c = transform.GetChild(1).GetComponent<TMP_Text>();
    }
    private void OnEnable()
    {
        StartCoroutine(ChangeColor());
    }
    IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(0.5f);
        c.color = c.color == Color.black ? Color.white : Color.black;
        StartCoroutine(ChangeColor());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Popup_Login.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
