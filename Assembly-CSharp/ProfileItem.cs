// Decompiled with JetBrains decompiler
// Type: ProfileItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
