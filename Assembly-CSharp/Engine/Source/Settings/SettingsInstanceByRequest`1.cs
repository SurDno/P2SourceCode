// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.SettingsInstanceByRequest`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Settings
{
  public abstract class SettingsInstanceByRequest<T> : InstanceByRequest<T> where T : class, new()
  {
    public SettingsInstanceByRequest() => SettingsViewService.AddSettings((object) this);

    protected virtual void OnInvalidate()
    {
    }

    public event Action OnApply;

    [Inspected]
    public void Apply()
    {
      this.OnInvalidate();
      Action onApply = this.OnApply;
      if (onApply == null)
        return;
      onApply();
    }
  }
}
