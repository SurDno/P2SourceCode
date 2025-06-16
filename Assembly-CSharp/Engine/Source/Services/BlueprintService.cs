using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using System;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (IBlueprintService)})]
  public class BlueprintService : IBlueprintService
  {
    public void Start(IBlueprintObject bp, IEntity owner, Action complete)
    {
      BlueprintServiceUtility.Start(bp, owner, complete, (string) null);
    }
  }
}
