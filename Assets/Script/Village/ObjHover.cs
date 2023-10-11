using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjHover : MonoBehaviour
{
  public RectTransform target;
  public Camera cam;
  public RectTransform text;
  private Vector2 sp;

  IEnumerator mouseOn()
  {
    while (true)
    {
      yield return new WaitForSeconds(0.2f);
      if (RectTransformUtility.RectangleContainsScreenPoint(target, Input.mousePosition, cam))
      {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(target, Input.mousePosition, cam, out sp);
        text.localPosition = sp;
      }
      
    }
  }

  private void OnMouseEnter()
  {
    text.gameObject.SetActive(true);
    StartCoroutine(mouseOn());
  }

  private void OnMouseExit()
  {
    text.gameObject.SetActive(false);
    StopCoroutine(mouseOn());
  }
}
