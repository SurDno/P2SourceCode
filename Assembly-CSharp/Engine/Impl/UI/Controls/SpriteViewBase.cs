// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SpriteViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class SpriteViewBase : SpriteView
  {
    [SerializeField]
    private Sprite value;

    public override Sprite GetValue() => this.value;

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;
      this.ApplyValue(true);
    }

    public override void SetValue(Sprite value, bool instant)
    {
      if ((Object) this.value == (Object) value)
        return;
      this.value = value;
      this.ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
