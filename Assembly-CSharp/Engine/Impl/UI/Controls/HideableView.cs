// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class HideableView : FloatView
  {
    [SerializeField]
    private bool visible = true;

    public event Action OnChangeEvent;

    public event Action OnSkipAnimationEvent;

    public event HideableView.VisibilityChanged OnVisibilityChanged;

    public bool Visible
    {
      get => this.visible;
      set
      {
        if (this.visible == value)
          return;
        this.visible = value;
        this.ApplyVisibility();
        Action onChangeEvent = this.OnChangeEvent;
        if (onChangeEvent != null)
          onChangeEvent();
        HideableView.VisibilityChanged visibilityChanged = this.OnVisibilityChanged;
        if (visibilityChanged == null)
          return;
        visibilityChanged(this.visible, this);
      }
    }

    public override void SkipAnimation()
    {
      Action skipAnimationEvent = this.OnSkipAnimationEvent;
      if (skipAnimationEvent == null)
        return;
      skipAnimationEvent();
    }

    public override float FloatValue
    {
      get => Convert.ToSingle(this.Visible);
      set => this.Visible = Convert.ToBoolean(value);
    }

    protected virtual void OnValidate() => this.ApplyVisibility();

    protected abstract void ApplyVisibility();

    public delegate void VisibilityChanged(bool newValue, HideableView view);
  }
}
