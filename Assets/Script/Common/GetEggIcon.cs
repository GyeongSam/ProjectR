using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GetEggIcon : MonoBehaviour
{
  public TMP_Text eggName;

  public void setEggInfo(int num)
  {
    eggName.text = "Egg"+num.ToString();
    GameObject obj = Instantiate(Resources.Load($"Village/Egg{num}") as GameObject, transform);
    obj.transform.localScale = new Vector3(obj.transform.localScale.x * 2, obj.transform.localScale.y * 2, obj.transform.localScale.z * 0.3f);
  }

}
