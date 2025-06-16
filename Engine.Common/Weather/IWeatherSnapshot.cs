using Engine.Common.Binders;
using Engine.Common.Blenders;

namespace Engine.Common.Weather
{
  [Sample("ISnapshot")]
  public interface IWeatherSnapshot : IBlendable<IWeatherSnapshot>, IObject
  {
  }
}
