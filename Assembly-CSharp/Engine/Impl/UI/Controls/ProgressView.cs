// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressView : ProgressViewBase
  {
    [SerializeField]
    [Range(0.0f, 1f)]
    private float progress = 0.0f;

    public override float Progress
    {
      get => this.progress;
      set
      {
        if ((double) this.progress == (double) value)
          return;
        this.progress = value;
        this.ApplyProgress();
      }
    }

    protected virtual void OnValidate() => this.ApplyProgress();

    protected abstract void ApplyProgress();
  }
}
