// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.GameLoadGameWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu.Main;
using System;
using System.Collections;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public class GameLoadGameWindow : CancelableSimpleWindow, IGameLoadGameWindow, IWindow, IPauseMenu
  {
    public static bool IsActive = false;

    public override IEnumerator OnOpened()
    {
      GameLoadGameWindow.IsActive = true;
      return base.OnOpened();
    }

    public override IEnumerator OnClosed()
    {
      GameLoadGameWindow.IsActive = false;
      return base.OnClosed();
    }

    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameLoadGameWindow>((IGameLoadGameWindow) this);
    }

    public override Type GetWindowType() => typeof (IGameLoadGameWindow);
  }
}
