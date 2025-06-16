// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.CommonTargets
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Services;
using System;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class CommonTargets
  {
    [Initialise]
    private static void RegisterTargets()
    {
      ConsoleTargetService.AddTarget("-player", (Func<string, object>) (value => (object) ServiceLocator.GetService<ISimulation>().Player));
      ConsoleTargetService.AddTarget("-slot", (Func<string, object>) (value => ServiceLocator.GetService<SelectionService>().GetSelection(DefaultConverter.ParseInt(value))));
    }
  }
}
