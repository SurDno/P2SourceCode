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

  public void SetText(string text) => GetComponent<Text>().text = text;

  public void OnPress()
  {
    Action<ProfileItem> onPressed = OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }

  public bool Slection
  {
    get => slection;
    set
    {
      slection = value;
      GetComponent<Text>().color = slection ? activeColor : baseColor;
    }
  }
}
