// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.EndCreditsWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu.Main;
using System;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public class EndCreditsWindow : CancelableSimpleWindow, IEndCreditsWindow, IWindow
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IEndCreditsWindow>((IEndCreditsWindow) this);
    }

    public override Type GetWindowType() => typeof (IEndCreditsWindow);
  }
}
