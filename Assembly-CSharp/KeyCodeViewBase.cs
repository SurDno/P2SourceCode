// Decompiled with JetBrains decompiler
// Type: KeyCodeViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class KeyCodeViewBase : KeyCodeView
{
  [SerializeField]
  private KeyCode value;

  private void OnValidate()
  {
    if (Application.isPlaying)
      return;
    this.ApplyValue(true);
  }

  public override KeyCode GetValue() => this.value;

  public override void SetValue(KeyCode value, bool instant)
  {
    if (this.value == value)
      return;
    this.value = value;
    this.ApplyValue(instant);
  }

  protected abstract void ApplyValue(bool instant);
}
