using System;
using Engine.Source.Services.Inputs;
using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour
{
  [SerializeField]
  private Color baseColor;
  [SerializeField]
  private Color activeColor;
  [SerializeField]
  private Color readonlyColor;
  private bool selction;
  public Action<KeyItem> OnPressed;

  public ActionGroup Bind { get; set; }

  public void SetText(string text) => GetComponent<Text>().text = text;

  public void SetReadonly() => GetComponent<Text>().color = readonlyColor;

  public bool Selection
  {
    get => selction;
    set
    {
      selction = value;
      GetComponent<Text>().color = selction ? activeColor : baseColor;
    }
  }

  public void OnPress()
  {
    Action<KeyItem> onPressed = OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }
}
