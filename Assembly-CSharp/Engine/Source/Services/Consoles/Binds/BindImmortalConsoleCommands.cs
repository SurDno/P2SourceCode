// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindImmortalConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindImmortalConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      SetConsoleCommand.AddBind<ParametersComponent, bool>("immortal", true, (Action<ParametersComponent, bool>) ((target, value) =>
      {
        IParameter<bool> byName = target.GetByName<bool>(ParameterNameEnum.Immortal);
        if (byName == null)
          return;
        byName.Value = value;
      }));
      GetConsoleCommand.AddBind<ParametersComponent, bool>("immortal", true, (Func<ParametersComponent, bool>) (target =>
      {
        IParameter<bool> byName = target.GetByName<bool>(ParameterNameEnum.Immortal);
        return byName != null && byName.Value;
      }));
    }
  }
}
