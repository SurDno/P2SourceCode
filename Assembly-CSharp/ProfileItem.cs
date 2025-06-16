using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileItem : MonoBehaviour
{
  [SerializeField]
  private Color baseColor;
  [SerializeField]
  private Color activeColor;
  private bool slection;
  public Action<ProfileItem> OnPressed;

  public string Name { get; set; }

  public void SetText(string text) => this.GetComponent<Text>().text = text;

  public void OnPress()
  {
    Action<ProfileItem> onPressed = this.OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }

  public bool Slection
  {
    get => this.slection;
    set
    {
      this.slection = value;
      this.GetComponent<Text>().color = this.slection ? this.activeColor : this.baseColor;
    }
  }
}
