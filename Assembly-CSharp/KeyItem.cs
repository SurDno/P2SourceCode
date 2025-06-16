using Engine.Source.Services.Inputs;
using System;
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

  public void SetText(string text) => this.GetComponent<Text>().text = text;

  public void SetReadonly() => this.GetComponent<Text>().color = this.readonlyColor;

  public bool Selection
  {
    get => this.selction;
    set
    {
      this.selction = value;
      this.GetComponent<Text>().color = this.selction ? this.activeColor : this.baseColor;
    }
  }

  public void OnPress()
  {
    Action<KeyItem> onPressed = this.OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }
}
