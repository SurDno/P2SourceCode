using System;
using Engine.Common.Commons;

namespace Engine.Common.Services
{
  public interface IBlueprintService
  {
    void Start(IBlueprintObject blueprint, IEntity owner, Action complete);
  }
}
