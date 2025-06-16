using System;

namespace Engine.Common.Components
{
  public interface IMarketComponent : IComponent
  {
    bool IsEnabled { get; set; }

    event Action OnFillPrices;
  }
}
