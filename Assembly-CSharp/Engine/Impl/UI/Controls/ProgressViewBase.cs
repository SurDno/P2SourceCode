// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressViewBase : FloatView
  {
    public abstract float Progress { get; set; }

    public override float FloatValue
    {
      get => this.Progress;
      set => this.Progress = Mathf.Clamp01(value);
    }
  }
}
