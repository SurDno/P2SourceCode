// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Utilities.UIServiceUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;
using System;

#nullable disable
namespace Engine.Source.Services.Utilities
{
  public static class UIServiceUtility
  {
    public static void PushWindow<T>(FlowOutput output, Action<T> initialise = null) where T : class, IWindow
    {
      UIService service = ServiceLocator.GetService<UIService>();
      T target = service.Get<T>();
      if (initialise != null)
        initialise(target);
      Action<IWindow> handler = (Action<IWindow>) null;
      handler = (Action<IWindow>) (window =>
      {
        target.DisableWindowEvent -= handler;
        output.Call();
      });
      ((T) target).DisableWindowEvent += handler;
      service.Push<T>();
    }
  }
}
