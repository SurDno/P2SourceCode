// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindTimeConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindTimeConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget(typeof (TimeService).Name, (Func<string, object>) (value => (object) ServiceLocator.GetService<ITimeService>()));
      SetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SolarTime = value));
      GetConsoleCommand.AddBind<TimeService, TimeSpan>("solar_time", true, (Func<TimeService, TimeSpan>) (target => target.SolarTime));
      SetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", false, (Action<TimeService, TimeSpan>) ((target, value) => target.SetGameTime(value)));
      GetConsoleCommand.AddBind<TimeService, TimeSpan>("game_time", true, (Func<TimeService, TimeSpan>) (target => target.GameTime));
    }
  }
}
