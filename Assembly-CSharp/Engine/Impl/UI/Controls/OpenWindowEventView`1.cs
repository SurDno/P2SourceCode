// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.OpenWindowEventView`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class OpenWindowEventView<T> : EventView where T : class, IWindow
  {
    [SerializeField]
    private bool swap = false;

    public override void Invoke()
    {
      if (!this.PrepareWindow())
        return;
      if (this.swap)
        ServiceLocator.GetService<UIService>().Swap<T>();
      else
        ServiceLocator.GetService<UIService>().Push<T>();
    }

    protected virtual bool PrepareWindow() => true;
  }
}
