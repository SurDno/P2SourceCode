using Engine.Common.Commons;
using System;

namespace Engine.Common.Services
{
  public interface IBlueprintService
  {
    void Start(IBlueprintObject blueprint, IEntity owner, Action complete);
  }
}
