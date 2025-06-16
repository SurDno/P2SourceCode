using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Services;
using System;

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
